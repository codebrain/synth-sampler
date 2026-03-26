using SynthStudio.Desktop.Models;

namespace SynthStudio.Desktop.Services;

public interface IAudioCaptureService
{
    Task<IReadOnlyList<AudioDeviceInfo>> GetDevicesAsync();
    Task<CaptureHandle> StartCaptureAsync(string deviceId, string outputPath, CancellationToken cancellationToken = default);
}

public sealed class CaptureHandle : IAsyncDisposable
{
    public string OutputPath { get; }

    public CaptureHandle(string outputPath)
    {
        OutputPath = outputPath;
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}
