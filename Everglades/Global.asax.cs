using Everglades.Controllers;
using Everglades.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

using System.Text;
using System.Threading.Tasks;


using APIFiMag;
using APIFiMag.Datas;
using APIFiMag.Exporter;
using APIFiMag.Importer;


namespace Everglades
{
    // Remarque : pour obtenir des instructions sur l'activation du mode classique IIS6 ou IIS7, 
    // visitez http://go.microsoft.com/?LinkId=9394801

    public class WebApiApplication : System.Web.HttpApplication
    {
        private ModelManage Mmodel;

        // debug Kevin
        //protected void Application_Start()
        protected void Application_Start(object sender, EventArgs e)
        {

            // debug kevin
            String _path = String.Concat(System.Environment.GetEnvironmentVariable("PATH"), ";", System.AppDomain.CurrentDomain.RelativeSearchPath);
            System.Environment.SetEnvironmentVariable("PATH", _path, EnvironmentVariableTarget.Process);

            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            /*
            //List<string> list_equities = new List<String> { "AAPL", "SAN", "0939.HK", "0941.HK", "CSGN.VX", "XOM", "HSBA.L", "1398.HK", "JNJ", "MSFT", "NESN.VX", "NOVN.VX", "PG", "ROG.VX", "SAN.PA", "SIE.DE", "TEF.TI", "FP.PA", "UBSG.VX", "VOD.L" };
            List<string> list_equities = new List<String> { "AAPL" };

            DateTime beginning = new DateTime(2015, 12, 1);
            DateTime end = DateTime.Now;
            string xmlfilePath = "C:/Users/ensimag/Desktop/Everglades/peps/Everglades/bin/YahooDataPeps.xml";
            List<HistoricalColumn> columns = new List<HistoricalColumn>();
            columns.Add(HistoricalColumn.Close);
            columns.Add(HistoricalColumn.High);
            columns.Add(HistoricalColumn.Open);
            columns.Add(HistoricalColumn.Volume);
            columns.Add(HistoricalColumn.Low);
            DataActif donnees = new DataActif(list_equities, columns, beginning, end);
            Environment.CurrentDirectory = "C:/Users/ensimag/Desktop/Everglades/peps/Everglades/bin";
            donnees.ImportData(new ImportYahoo());
            donnees.Export(new ExportXML(xmlfilePath));
            */



            //HomeController.Yahoo_Finance_Parsing();
            Mmodel = new ModelManage();
            Application["Mmodel"] = Mmodel;


        }

        /*void Application_Start(object sender, EventArgs e)
        {
            String _path = String.Concat(System.Environment.GetEnvironmentVariable("PATH"), ";", System.AppDomain.CurrentDomain.RelativeSearchPath);
            System.Environment.SetEnvironmentVariable("PATH", _path, EnvironmentVariableTarget.Process);
        }*/
    }
}