using System.Text.Json;
using Microsoft.Extensions.Logging;
using SynthStudio.Desktop.Bridge;
using SynthStudio.Desktop.Models;

namespace SynthStudio.Desktop.Services;

public sealed class AutomationJobService
{
    private readonly IMidiService _midiService;
    private readonly IAudioCaptureService _audioCaptureService;
    private readonly IStorageService _storageService;
    private readonly BridgeMessageDispatcher _bridge;
    private readonly ILogger<AutomationJobService> _logger;

    private CancellationTokenSource? _activeRun;

    public AutomationJobService(
        IMidiService midiService,
        IAudioCaptureService audioCaptureService,
        IStorageService storageService,
        BridgeMessageDispatcher bridge,
        ILogger<AutomationJobService> logger)
    {
        _midiService = midiService;
        _audioCaptureService = audioCaptureService;
        _storageService = storageService;
        _bridge = bridge;
        _logger = logger;
    }

    public async Task<string> StartBatchAsync(BatchCaptureRequest request)
    {
        if (_activeRun is not null)
        {
            throw new InvalidOperationException("A batch run is already active.");
        }

        var jobId = Guid.NewGuid().ToString("N");
        _activeRun = new CancellationTokenSource();
        var cancellationToken = _activeRun.Token;

        _ = Task.Run(async () =>
        {
            try
            {
                var runFolder = await _storageService.EnsureRunFolderAsync(
                    request.OutputFolder,
                    request.SynthProfileId,
                    cancellationToken);

                await _bridge.EmitAsync("jobs:started", new { jobId, runFolder });

                var manifest = new
                {
                    jobId,
                    request,
                    startedAtUtc = DateTime.UtcNow,
                    renders = new List<object>()
                };

                for (int patchIndex = 0; patchIndex < request.SysExFiles.Count; patchIndex++)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var patchPath = request.SysExFiles[patchIndex];
                    var patchBytes = await File.ReadAllBytesAsync(patchPath, cancellationToken);

                    await _bridge.EmitAsync("jobs:progress", new JobProgressEvent
                    {
                        JobId = jobId,
                        CurrentPatchIndex = patchIndex + 1,
                        PatchCount = request.SysExFiles.Count,
                        CurrentPatternIndex = 0,
                        PatternCount = request.MidiPatternFiles.Count,
                        Stage = "sysex",
                        Message = $"Sending patch {Path.GetFileName(patchPath)}"
                    });

                    await _midiService.SendSysExAsync(request.MidiOutputId, patchBytes, cancellationToken);
                    await Task.Delay(request.SettleDelayMs, cancellationToken);

                    for (int patternIndex = 0; patternIndex < request.MidiPatternFiles.Count; patternIndex++)
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        var patternPath = request.MidiPatternFiles[patternIndex];
                        var outputPath = await _storageService.BuildRenderPathAsync(
                            runFolder,
                            Path.GetFileNameWithoutExtension(patchPath),
                            Path.GetFileNameWithoutExtension(patternPath),
                            cancellationToken);

                        await _bridge.EmitAsync("jobs:progress", new JobProgressEvent
                        {
                            JobId = jobId,
                            CurrentPatchIndex = patchIndex + 1,
                            PatchCount = request.SysExFiles.Count,
                            CurrentPatternIndex = patternIndex + 1,
                            PatternCount = request.MidiPatternFiles.Count,
                            Stage = "recording",
                            Message = $"Recording {Path.GetFileName(patternPath)}"
                        });

                        await using var capture = await _audioCaptureService.StartCaptureAsync(
                            request.AudioInputId,
                            outputPath,
                            cancellationToken);

                        await _midiService.PlayMidiPatternAsync(request.MidiOutputId, patternPath, cancellationToken);
                        await Task.Delay(request.TailMs, cancellationToken);
                    }
                }

                await _storageService.WriteManifestAsync(runFolder, manifest, cancellationToken);
                await _bridge.EmitAsync("jobs:completed", new { jobId, runFolder });
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Batch run cancelled.");
                await _bridge.EmitAsync("jobs:failed", new { jobId, error = "Cancelled" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Batch run failed.");
                await _bridge.EmitAsync("jobs:failed", new { jobId, error = ex.Message });
            }
            finally
            {
                _activeRun?.Dispose();
                _activeRun = null;
            }
        }, cancellationToken);

        return jobId;
    }

    public Task CancelAsync()
    {
        _activeRun?.Cancel();
        return Task.CompletedTask;
    }
}
