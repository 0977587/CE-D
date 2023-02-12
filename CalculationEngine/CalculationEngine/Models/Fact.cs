using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CalculationEngine.Models
{
    public class Fact
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("value")]
        public float Value { get; set; }
        [JsonPropertyName("rulenames")]
        public List<string> RuleNames { get; set; }
        [JsonPropertyName("operators")]
        public List<string> Operators { get; set; }
    }
}