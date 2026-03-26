namespace SynthStudio.Desktop.Models;

public sealed class BatchCaptureRequest
{
    public string SynthProfileId { get; set; } = string.Empty;
    public string MidiOutputId { get; set; } = string.Empty;
    public string AudioInputId { get; set; } = string.Empty;
    public string OutputFolder { get; set; } = "renders";
    public int SettleDelayMs { get; set; } = 1500;
    public int TailMs { get; set; } = 1000;
    public List<string> SysExFiles { get; set; } = new();
    public List<string> MidiPatternFiles { get; set; } = new();
}
