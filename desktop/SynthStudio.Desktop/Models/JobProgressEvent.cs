namespace SynthStudio.Desktop.Models;

public sealed class JobProgressEvent
{
    public string JobId { get; set; } = string.Empty;
    public int CurrentPatchIndex { get; set; }
    public int PatchCount { get; set; }
    public int CurrentPatternIndex { get; set; }
    public int PatternCount { get; set; }
    public string Stage { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
