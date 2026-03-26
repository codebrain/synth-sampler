using System.Text.Json;
using SynthStudio.Desktop.Models;

namespace SynthStudio.Desktop.Services;

public sealed class ProfileService
{
    private readonly string _profileDirectory;

    public ProfileService()
    {
        _profileDirectory = Path.Combine(AppContext.BaseDirectory, "Profiles");
    }

    public async Task<IReadOnlyList<SynthProfile>> GetProfilesAsync()
    {
        if (!Directory.Exists(_profileDirectory))
        {
            return Array.Empty<SynthProfile>();
        }

        var profiles = new List<SynthProfile>();
        foreach (var file in Directory.GetFiles(_profileDirectory, "*.json"))
        {
            var json = await File.ReadAllTextAsync(file);
            var profile = JsonSerializer.Deserialize<SynthProfile>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (profile is not null)
            {
                profiles.Add(profile);
            }
        }

        return profiles;
    }
}
