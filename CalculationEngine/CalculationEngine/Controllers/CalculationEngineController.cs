
using CalculationEngine.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Xml.Serialization;
using Microsoft.CodeAnalysis;
using System.Collections;

namespace CalculationEngine.Controllers
{   
    public class CalculationEngineController : Controller
    {
        CalcEngine calcengine =  new CalcEngine();

        public CalculationEngineController()
        {
            calcengine = getDatabase();
        }

        [HttpGet]
        // GET: CalculationEngineController
        public ActionResult Index()
        {
            return View();
        }
        // GET: CalculationEngineController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: CalculationEngineController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CalculationEngineController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: CalculationEngineController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: CalculationEngineController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: CalculationEngineController/Delete/5
        public ActionResult Delete(int id){
            return View();
        }

        // POST: CalculationEngineController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
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

        [HttpGet]
        // GET: CalculationEngine/GetNode/{Path}
        public Node GetNode(string path)
        {
            var result = calcengine.Nodes.Find(node => node.Name == path);
            return result;
        }
        [HttpGet]
        // GET: CalculationEngine/GetData
        public List<string> GetData()
        {
            return calcengine.Paths;
        }

        [HttpGet]
        // GET: CalculationEngine/GetNodeData
        public List<string> GetNodeData()
        {
            
            var result = mapFrom(calcengine.Nodes);
            List<string> string2 = new List<string>();
            foreach(var res in result)
            {
                string jsonString = JsonSerializer.Serialize(res);
                string2.Add(jsonString);
            }
            
            return string2;
        }

        [HttpGet]
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
