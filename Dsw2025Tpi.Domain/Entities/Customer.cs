using System.Text.Json.Serialization;

public class Customer
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("email")]
    public string Email { get; set; } = "";
}
