using System;
using System.Text.Json.Serialization;

namespace CalculationEngine.Models
{
    public class RuleCondition
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("body")]
        public string Value { get; set; }
    }
}


