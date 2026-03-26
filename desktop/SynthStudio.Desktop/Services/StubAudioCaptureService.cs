using Microsoft.Extensions.Logging;
using SynthStudio.Desktop.Models;

namespace SynthStudio.Desktop.Services;

public sealed class StubAudioCaptureService : IAudioCaptureService
{
    private readonly ILogger<StubAudioCaptureService> _logger;

    public StubAudioCaptureService(ILogger<StubAudioCaptureService> logger)
    {
        _logger = logger;
    }

    public Task<IReadOnlyList<AudioDeviceInfo>> GetDevicesAsync()
    {
        IReadOnlyList<AudioDeviceInfo> devices =
        [
            new AudioDeviceInfo { Id = "line34", Name = "Audio Interface Line In 3/4", Channels = 2 }
        ];

        return Task.FromResult(devices);
    }

    public Task<CaptureHandle> StartCaptureAsync(string deviceId, string outputPath, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Stub starting capture from {DeviceId} to {Path}", deviceId, outputPath);

        Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
        File.WriteAllText(outputPath, "Placeholder WAV file. Replace with real capture implementation.");

        return Task.FromResult(new CaptureHandle(outputPath));
    }
}
