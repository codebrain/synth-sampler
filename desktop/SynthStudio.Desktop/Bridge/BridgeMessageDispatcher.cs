using System.Text.Json;
using Microsoft.Extensions.Logging;
using SynthStudio.Desktop.Models;
using SynthStudio.Desktop.Services;

namespace SynthStudio.Desktop.Bridge;

public sealed class BridgeMessageDispatcher
{
    private readonly IMidiService _midiService;
    private readonly IAudioCaptureService _audioService;
    private readonly ProfileService _profileService;
    private readonly AutomationJobService _jobService;
    private readonly ILogger<BridgeMessageDispatcher> _logger;

    public event EventHandler<string>? HostEventProduced;

    public BridgeMessageDispatcher(
        IMidiService midiService,
        IAudioCaptureService audioService,
        ProfileService profileService,
        IServiceProvider services,
        ILogger<BridgeMessageDispatcher> logger)
    {
        _midiService = midiService;
        _audioService = audioService;
        _profileService = profileService;
        _jobService = services.GetRequiredService<AutomationJobService>();
        _logger = logger;
    }

    public async Task<string?> DispatchAsync(string rawJson)
    {
        var envelope = JsonSerializer.Deserialize<BridgeEnvelope>(rawJson, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (envelope is null)
        {
            return null;
        }

        _logger.LogInformation("Bridge request {Type}", envelope.Type);

        return envelope.Type switch
        {
            "app:get-state" => Reply(envelope, new { ready = true, mode = "desktop" }),
            "midi:get-devices" => Reply(envelope, await _midiService.GetDevicesAsync()),
            "audio:get-devices" => Reply(envelope, await _audioService.GetDevicesAsync()),
            "profiles:get" => Reply(envelope, await _profileService.GetProfilesAsync()),
            "jobs:start-batch" => await StartBatchAsync(envelope),
            "jobs:cancel" => await CancelBatchAsync(envelope),
            _ => Reply(envelope, new { error = $"Unknown message type '{envelope.Type}'." }, "error")
        };
    }

    public Task EmitAsync(string type, object payload)
    {
        var json = JsonSerializer.Serialize(new
        {
            id = Guid.NewGuid().ToString("N"),
            type,
            direction = "event",
            payload
        });

        HostEventProduced?.Invoke(this, json);
        return Task.CompletedTask;
    }

    private async Task<string> StartBatchAsync(BridgeEnvelope envelope)
    {
        var request = envelope.Payload.Deserialize<BatchCaptureRequest>(new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? new BatchCaptureRequest();

        var jobId = await _jobService.StartBatchAsync(request);
        return Reply(envelope, new { jobId });
    }

    private async Task<string> CancelBatchAsync(BridgeEnvelope envelope)
    {
        await _jobService.CancelAsync();
        return Reply(envelope, new { cancelled = true });
    }

    private static string Reply(BridgeEnvelope request, object payload, string direction = "response")
    {
        return JsonSerializer.Serialize(new
        {
            id = request.Id,
            type = request.Type,
            direction,
            payload
        });
    }
}
