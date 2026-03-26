using System.Text.Json;

namespace SynthStudio.Desktop.Services;

public sealed class FileStorageService : IStorageService
{
    public Task<string> EnsureRunFolderAsync(string baseFolder, string synthId, CancellationToken cancellationToken = default)
    {
        var runFolder = Path.Combine(
            Path.GetFullPath(baseFolder),
            synthId,
            DateTime.UtcNow.ToString("yyyyMMdd_HHmmss"));

        Directory.CreateDirectory(runFolder);
        return Task.FromResult(runFolder);
    }

    public Task<string> BuildRenderPathAsync(string runFolder, string patchName, string patternName, CancellationToken cancellationToken = default)
    {
        var safePatch = Sanitize(patchName);
        var safePattern = Sanitize(patternName);
        var patchFolder = Path.Combine(runFolder, safePatch);
        Directory.CreateDirectory(patchFolder);

        var outputPath = Path.Combine(patchFolder, $"{safePattern}.wav");
        return Task.FromResult(outputPath);
    }

    public async Task WriteManifestAsync(string runFolder, object manifest, CancellationToken cancellationToken = default)
    {
        var path = Path.Combine(runFolder, "manifest.json");
        await File.WriteAllTextAsync(
            path,
            JsonSerializer.Serialize(manifest, new JsonSerializerOptions { WriteIndented = true }),
            cancellationToken);
    }

    private static string Sanitize(string name)
    {
        foreach (var c in Path.GetInvalidFileNameChars())
        {
            name = name.Replace(c, '_');
        }

        return string.IsNullOrWhiteSpace(name) ? "unnamed" : name;
    }
}
