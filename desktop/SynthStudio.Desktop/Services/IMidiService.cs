using SynthStudio.Desktop.Models;

namespace SynthStudio.Desktop.Services;

public interface IMidiService
{
    Task<IReadOnlyList<MidiDeviceInfo>> GetDevicesAsync();
    Task SendSysExAsync(string outputId, byte[] data, CancellationToken cancellationToken = default);
    Task PlayMidiPatternAsync(string outputId, string midiFilePath, CancellationToken cancellationToken = default);
}
