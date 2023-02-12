
using CalculationEngine.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CalculationEngineDashboard.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CalculationEngineController : ControllerBase
    {
        CalcEngine calcengine =  new CalcEngine();


        // GET: CalculationEngine/GetData
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return calcengine.Paths;
        }


       
        public CalcEngine getDatabase()
        {
            string root = Directory.GetCurrentDirectory();
            string subdir = root + "\\Data";
            // If directory does not exist, create it.  
            if (!Directory.Exists(subdir))
            {
                Directory.CreateDirectory(subdir);
                return new CalcEngine();
            }
            
            var jsonString = subdir + "\\data.json";
            if (System.IO.File.Exists(jsonString))
            {
                var fileJson = System.IO.File.ReadAllText(jsonString);
                CalcEngine calculationEngine = JsonSerializer.Deserialize<CalcEngine>(fileJson);
                foreach(var node in calculationEngine.Nodes)
                {
                    node.Dependencies = new List<string>();
                    node.Rules = new List<Rule>();
                    node.RuleNames = new List<string>();
                }
                calculationEngine.MaxNodes = calculationEngine.Nodes.Count();
                calculationEngine.Paths = calculationEngine.GetPaths();
                calculationEngine.SetDescriptions();
                calculationEngine.SetFacts();
                calculationEngine.SetRules();
                calculationEngine.calculateVisualisation();
                return calculationEngine;
            }
            Directory.CreateDirectory(jsonString);
            return new CalcEngine();

        }

        public string getCSV()
        {
            string root = Directory.GetCurrentDirectory();
            string subdir = root + "\\Data";
            // If directory does not exist, create it.  
            if (!Directory.Exists(subdir))
            {
                Directory.CreateDirectory(subdir);
                return "";
            }

            var csvString = subdir + "\\csvdata.csv";
            if (System.IO.File.Exists(csvString))
            {
                var fileJson = System.IO.File.ReadAllText(csvString);
                return fileJson;
            }
            
            Directory.CreateDirectory(csvString);
            return csvString;
        }

        private List<frontNode> mapFrom(List<Node> nodes)
        {
            List<frontNode> result = new List<frontNode>();
            foreach(var node in nodes)
            {
                var resNode = node.MapTo();
                result.Add(resNode);
            }
            return result;
        }
        private List<frontLink> mapFrom(List<Link> links)
        {
            List<frontLink> result = new List<frontLink>();
            foreach (var link in links)
            {
                result.Add(link.MapTo());
            }
            return result;
        }

     
        // GET: CalculationEngine/GetNode/{Path}
        public Node GetNode(string path)
        {
            return calcengine.Nodes.Find(node => node.Name == path);
        }
       
       

        
        // GET: CalculationEngine/GetNodeData
        public IEnumerable<string> GetNodeData()
        {
            
            var result = mapFrom(calcengine.Nodes);
            List<string> string2 = new List<string>();
            foreach(var res in result)
            {
                string jsonString = JsonSerializer.Serialize(res);
                string2.Add(jsonString);
            }
            
            return string2.ToArray();
        }



        // GET: CalculationEngine/GetLinkData
        public List<string> GetLinkData()
        {
            List<string> string2 = new List<string>();
            foreach (var link in calcengine.Links)
            {
                string jsonString = JsonSerializer.Serialize(link);
                string2.Add(jsonString);
            }
            var rootNode = calcengine.FindRootNodeByName();
            var reslink = new Link(0, rootNode.Id);
            var rootString = JsonSerializer.Serialize(reslink);
            string2.Add(rootString);

            return string2;
        }
    }
}
