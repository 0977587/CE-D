using System.Text.Json.Serialization;

namespace CalculationEngine.Models
{
    public class Description
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("body")]
        public string Body { get; set; }
    }
}
