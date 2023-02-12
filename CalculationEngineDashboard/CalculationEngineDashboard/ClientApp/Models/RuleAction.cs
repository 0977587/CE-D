using System.Text.Json.Serialization;

namespace CalculationEngine.Models
{
    public class RuleAction
    {
        [JsonPropertyName("event")]
        public string Event { get; set; }
        [JsonPropertyName("body")]
        public string Body { get; set; }

    }
}