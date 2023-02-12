using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CalculationEngine.Models
{


    [ExcludeFromCodeCoverage]

    public class Rule
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("rulecondition")]
        public RuleCondition RuleCondition { get; set; }
        [JsonPropertyName("ruleaction")]
        public RuleAction RuleAction { get; set; }
        

}
}
