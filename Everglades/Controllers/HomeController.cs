using Everglades.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
/*
using APIFiMag;
using APIFiMag.Datas;
using APIFiMag.Exporter;
using APIFiMag.Importer;
*/
  
namespace Everglades.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View(HttpContext.Application["Mmodel"]);
        }

        /*
        public static bool Yahoo_Finance_Parsing()
        {
            List<string> symbol = new List<string>() { "EDF.PA", "EN.PA", "BNP.PA", "1398.HK", "JNJ",  "MSFT","NESNE.SW","NOVNEE.SW","PG","ROG.VX" };
            List<HistoricalColumn> columns = new List<HistoricalColumn>();
            columns.Add(HistoricalColumn.Close);
            columns.Add(HistoricalColumn.High);
            columns.Add(HistoricalColumn.Open);
            columns.Add(HistoricalColumn.Volume);
            columns.Add(HistoricalColumn.Low);
            DateTime debut = new DateTime(2015, 12, 06);
            DateTime fin = new DateTime(2015, 12, 08);
            DataActif donnees = new DataActif(symbol, columns, debut, fin);
            Environment.CurrentDirectory = "C:/Users/ensimag/Desktop/peps/peps/Everglades/bin";
            donnees.ImportData(new ImportYahoo());
            donnees.Export(new ExportJSON("donnees.json"));
            //System.Console.WriteLine(donnees.ToString());
            return false;
        }
        */
        
        public bool Buy_Asset()
        {
            return false;
        }
    
    }
}
