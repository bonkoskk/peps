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

namespace Everglades
{
    // Remarque : pour obtenir des instructions sur l'activation du mode classique IIS6 ou IIS7, 
    // visitez http://go.microsoft.com/?LinkId=9394801

    public class WebApiApplication : System.Web.HttpApplication
    {
        private ModelManage Mmodel;

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            HomeController.Yahoo_Finance_Parsing();
            Mmodel = new ModelManage();
            IAsset asset1 = new Equity("gougeul");
            Mmodel.Assets.Add_Asset(asset1, 4);
            IAsset asset2 = new Equity("apeul");
            Mmodel.Assets.Add_Asset(asset2, 3);
            IAsset asset3 = new Equity("mikrosofte");
            Mmodel.Assets.Add_Asset(asset3, 6);
            IAsset asset4 = new Equity("startupdekevintheoetbaptiste");
            Mmodel.Assets.Add_Asset(asset4, 3);
            for (uint i = 5; i <= 20; i++)
            {
                IAsset asseti = new Equity("action" + i);
                Mmodel.Assets.Add_Asset(asseti, i);
            }
            Application["Mmodel"] = Mmodel;
        }
    }
}