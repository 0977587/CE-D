
using System.Data;
using System.Text;
using System.Text.Json.Serialization;

namespace CalculationEngine.Models
{
    static class Extensions
    {
        public static T RemoveFirst<T>(this ICollection<T> items)
        {
            T item = items.FirstOrDefault();
            if (item != null)
            {
                items.Remove(item);
            }
            return item;
        }
    }

    public class CalcEngine
    {
        [JsonPropertyName("rules")]
        public List<Rule> Rules { get; set; }

        [JsonPropertyName("facts")]
        public List<Fact> Facts { get; set; }

        [JsonPropertyName("nodes")]
        public List<Node> Nodes { get; set; }

        [JsonPropertyName("links")]
        public List<Link> Links { get; set; }

        [JsonPropertyName("descriptions")]
        public List<Description> Descriptions { get; set; }

        [JsonPropertyName("isChanged")]
        public bool DataChanged { get; set; }

        public List<string> Paths { get; set; }

        public Node RootNode { get; set; }
        public int MaxNodes { get; set; }

        public List<string> GetPaths()
        {
            List<string> Paths = new List<string>();
            int count = Links.Count;
            for (int i = 0; i < count; i++)
            {
                var result = GetPath(Links[i]);
                Paths.Add(result);
            }
            Node root = FindRootNode();
            root.Path = root.Name;
            Paths.Insert(0, root.Name);
            return Paths;
        }

            
        public Node FindRootNode()
        {
            return Nodes.Where(x => string.IsNullOrEmpty(x.Path)).FirstOrDefault();
        }

        public Node FindRootNodeByName()
        {
            return Nodes.Where(x => x.Name == x.Path).FirstOrDefault();
        }

        public string GetPath(Link link)
        {
            var sourceNode = Getnode(link.Source);  
            var targetNode = Getnode(link.Target);

            sourceNode.Dependencies.Add(targetNode.Name);

            var uniqueItems = sourceNode.Dependencies.Distinct().ToList();
            sourceNode.Dependencies = uniqueItems;
            if (!string.IsNullOrEmpty(sourceNode.Path))
            {
                targetNode.Path = sourceNode.Path + "/" + targetNode.Name;
                return sourceNode.Path + "/" + targetNode.Name;
            }
            else
            {
                targetNode.Path = sourceNode.Name + "/" + targetNode.Name;
                return sourceNode.Name + "/" + targetNode.Name;
            }
        }

        public Node Getnode( int To_Find){
            return Nodes.Where(x =>x.Id == To_Find).FirstOrDefault();
        }

        public void SetFacts()
        {
            foreach (var fact in Facts)
            {
                SetFact(fact);
            }
        }
        private void SetFact(Fact fact)
        {
            Node targetNode = Nodes.Where(x => x.Id == fact.Id).FirstOrDefault();
            if (!fact.Value.Equals(null))
            {
                targetNode.Value = fact.Value;
            }
            if (fact.Operators != null)
            {
                targetNode.Operators = fact.Operators;
            }   

            if (fact.RuleNames != null & fact.RuleNames.Count() > 0)
            {
                foreach(var rulename in fact.RuleNames)
                {
                    targetNode.RuleNames.Add(rulename);
                }
            }
        }

        public void SetRules()
        {
            foreach (var rule in Rules)
            {
                SetRule(rule);
            }
        }
        private void SetRule(Rule rule)
        {   
            foreach(var node in Nodes)
            {
                foreach(var rulenames in node.RuleNames)
                {
                    if(rulenames == rule.Name)
                    {
                        node.Rules.Add(rule);
                    }
                }
            }
        }
        public void SetDescriptions()
        {
            foreach (var description in Descriptions)
            {
                SetDescription(description);
            }
        }

        private void SetDescription(Description description)
        {
            Node targetNode = Nodes.Where(x => x.Id == description.Id).FirstOrDefault();
            targetNode.Description = description.Body;
        }
        private void writetoCSV()
        {
            string root = Directory.GetCurrentDirectory();
            string subdir = root + "\\Data";
            var csvString = subdir + "\\csvdata.csv";
            if (!File.Exists(csvString))
            {
                //before your loop
                var csv = new StringBuilder();
                csv.AppendLine("path");
                foreach (var path in Paths)
                {
                    var first = path.ToString();
                    var newLine = string.Format("{0}", first);
                    csv.AppendLine(newLine);
                }
                //after your loop
                File.WriteAllText(csvString, csv.ToString());
            }
        }
      
        public static bool IsEmpty<T>(List<T> list)
        {
            if (list == null)
            {
                return true;
            }

            return list.FirstOrDefault() != null;
        }


        internal void calculateVisualisation()
        {
            var rootNode = FindRootNodeByName();
            if (rootNode.Value.HasValue && rootNode.Value!=0)
            {
                return;
            }
            HashSet<Node> NodestoTravel = new HashSet<Node>();
            HashSet<Node> Visited = new HashSet<Node>();

            NodestoTravel.Add(rootNode);

            while (NodestoTravel.Count() > 0)
            {
                var node = NodestoTravel.FirstOrDefault();
                NodestoTravel.Remove(node);
                if (Visited.Contains(node))
                    continue;
                Visited.Add(node);
                foreach (var dependency in node.Dependencies)
                {
                    var childNode = GetNodeByName(dependency);
                    
                    if (!Visited.Contains(childNode))
                        NodestoTravel.Add(childNode);
                }
                if (node.Value != 0)
                {
                    continue;
                }
                else
                {
                    node.Value = Solve(node);
                }

            }
            string s = rootNode.expressionString;
            var expr = MathExpressionGenerator.GetExpression(s);
            rootNode.expressionString = expr.Body.ToString();
        }

        private float? Solve(Node node)
        {
            if (node.Value != 0)
            {
                return node.Value;
            }
            float result = 0;
            var index = 0;
            string last = node.Dependencies.Last();
            string expression = "";
            foreach (var dependency in node.Dependencies)
            {
                var childNode = GetNodeByName(dependency);
                if (childNode.Value.HasValue || childNode.Value != 0)
                {
                    childNode.Value = Solve(childNode);
                }
                if (node.Operators.Count() == 1)
                {
                    var calc = new Calculation()
                    {
                        LeftHandValue = new Calculation() { CurrentValue = result },
                        RightHandValue = new Calculation() { CurrentValue = childNode.Value }
                    };
                    var operation = node.Operators.FirstOrDefault();
                    calc.setOperation(operation);
                    result = calc.Solve();

                    if (dependency.Equals(last))
                    {
                        expression += childNode.Value.ToString();
                        node.expressionString = expression;
                        return result;
                    }
                    else
                    {
                        expression += childNode.Value.ToString() + calc.getOperation(operation);// do something different with the last item
                    }
                }
                else
                {
                    var childNode2 = GetNodeByName(node.Dependencies[index]);
                    if (childNode2.Value == 0)
                    {
                        childNode2.Value = Solve(childNode2);
                    }
                    var calc = new Calculation()
                    {
                        LeftHandValue = new Calculation() { CurrentValue = result },
                        RightHandValue = new Calculation()
                        {
                            LeftHandValue = new Calculation() { CurrentValue = childNode.Value },
                            RightHandValue = new Calculation() { CurrentValue = childNode2.Value },
                            Function = (left, right) => left + right
                        },
                        Function = (left, right) => left + right
                    };
                    var operation = node.Operators.FirstOrDefault();
                    calc.RightHandValue.setOperation(operation);
                    node.Operators.RemoveFirst();
                    result = calc.Solve();
                    if (dependency.Equals(last))
                    {
                        expression += childNode.Value.ToString();
                        node.expressionString = expression;
                        return result;
                    }
                    else
                    {
                        expression += childNode.Value.ToString() + calc.getOperation(operation); ;
                    }
                    index++;
                }
            }
            return result;
        }

       

        private Node GetNodeByName(string dependency)
        {
            return Nodes.Where(x => x.Name == dependency).FirstOrDefault();
        }
    }
}
