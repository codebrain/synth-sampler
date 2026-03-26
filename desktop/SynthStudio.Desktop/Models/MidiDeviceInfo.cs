namespace SynthStudio.Desktop.Models;

public sealed class MidiDeviceInfo
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsInput { get; set; }
    public bool IsOutput { get; set; }
}
