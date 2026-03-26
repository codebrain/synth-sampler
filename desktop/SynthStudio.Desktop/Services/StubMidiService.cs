using Microsoft.Extensions.Logging;
using SynthStudio.Desktop.Models;

namespace SynthStudio.Desktop.Services;

public sealed class StubMidiService : IMidiService
{
    private readonly ILogger<StubMidiService> _logger;

    public StubMidiService(ILogger<StubMidiService> logger)
    {
        _logger = logger;
    }

    public Task<IReadOnlyList<MidiDeviceInfo>> GetDevicesAsync()
    {
        IReadOnlyList<MidiDeviceInfo> devices =
        [
            new MidiDeviceInfo { Id = "rtp-1", Name = "RTP MIDI Session 1", IsOutput = true, IsInput = true },
            new MidiDeviceInfo { Id = "usb-oberheim", Name = "OB-X8 USB MIDI", IsOutput = true, IsInput = true }
        ];

        return Task.FromResult(devices);
    }

    public Task SendSysExAsync(string outputId, byte[] data, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Stub SysEx send to {OutputId}, {Length} bytes", outputId, data.Length);
        return Task.CompletedTask;
    }

    public async Task PlayMidiPatternAsync(string outputId, string midiFilePath, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Stub playing MIDI pattern {Pattern} to {OutputId}", midiFilePath, outputId);
        await Task.Delay(1200, cancellationToken);
    }
}
