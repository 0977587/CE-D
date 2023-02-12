using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace CalculationEngine.Models
{
    public class Link
    {
        public Link(int source, int target)
        {
            Source = source;
            Target = target;
        }

        [JsonPropertyName("source")]
        public int Source { get; set; }
        [JsonPropertyName("target")]
        public int Target { get; set; }

        public frontLink MapTo()
        {
            return new frontLink(Source.ToString(), Target.ToString());
        }
    }
    public class LinkString
    {
        public LinkString(string source, string target)
        {
            Source = source;
            Target = target;
        }

        [JsonPropertyName("source")]
        public string Source { get; set; }
        [JsonPropertyName("target")]
        public string Target { get; set; }

    }

    public class frontLink
    {
        public frontLink(string source, string target)
        {
            Source = source;
            Target = target;
        }

       
        public string Source { get; set; }
        public string Target { get; set; }
    }

}