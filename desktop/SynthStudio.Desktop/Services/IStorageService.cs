using SynthStudio.Desktop.Models;

namespace SynthStudio.Desktop.Services;

public interface IStorageService
{
    Task<string> EnsureRunFolderAsync(string baseFolder, string synthId, CancellationToken cancellationToken = default);
    Task<string> BuildRenderPathAsync(string runFolder, string patchName, string patternName, CancellationToken cancellationToken = default);
    Task WriteManifestAsync(string runFolder, object manifest, CancellationToken cancellationToken = default);
}
