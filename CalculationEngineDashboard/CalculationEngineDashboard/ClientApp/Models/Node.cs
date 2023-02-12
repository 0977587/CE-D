
using System.Text.Json.Serialization;

namespace CalculationEngine.Models
{

    public class Node
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("rulenames")]
        public List<string> RuleNames { get; set; }
        
        public List<string> Operators { get; set; }

        public string Path { get; set; }
        public float? Value { get; set; }
        public string Description { get; set; }
        public List<Calculation> Calculations { get; set; }

        public List<Rule> Rules { get; set; }
        public List<string> Dependencies { get; set; }
        public string expressionString { get; set; }
        public frontNode MapTo()
        {
            var levels = Path.Split('/');
            var level = levels.Count() - 1;
            var module = level > 0 ? levels[1] : null;

            var leaf = levels.Last();
            var tempparent = levels.SkipLast(1).ToArray();
            var parent = string.Join(",", tempparent);
            if (parent.Count() == 0)
            {
                Path = levels.Last();
                parent = levels.Last();
            }

            var moduleres = 0;
            int x = 0;

            if (Int32.TryParse(module, out x))
            {
                moduleres = x;
            }
            else
            {
                moduleres = 0;
            }

            var result = new frontNode(Path, leaf, moduleres, Id, parent, level);
            return result;
        }

       
    }
    public class frontNode
    {
        public frontNode(string path, string leaf, int module, int id, string parent, int level)
        {
            Path = path;
            Leaf = leaf;
            Module = module;
            Id = id;
            Parent = parent;
            Size = 20;
            Level = level;
        }

        public string Path { get; set; }

        public string Leaf { get; set; }
        public int Module { get; set; }
        public int Id { get; set; }
        public string Parent { get; set; }
        public int Level { get; set; }
        public int Size { get; set; }
    }

    
}
