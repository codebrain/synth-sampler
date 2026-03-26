namespace SynthStudio.Desktop.Models;

public sealed class SynthParameter
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = "cc";
    public int Number { get; set; }
    public int Min { get; set; }
    public int Max { get; set; } = 127;
}
