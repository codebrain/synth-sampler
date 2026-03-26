using System.Text.Json;
using System.Text.Json.Serialization;

namespace SynthStudio.Desktop.Models;

public sealed class BridgeEnvelope
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("direction")]
    public string Direction { get; set; } = "request";

    [JsonPropertyName("payload")]
    public JsonElement Payload { get; set; }
}
