namespace SynthStudio.Desktop.Models;

public sealed class SynthProfile
{
    public string Id { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Manufacturer { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public List<SynthParameter> Parameters { get; set; } = new();
}
