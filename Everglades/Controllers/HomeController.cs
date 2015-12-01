using Everglades.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Everglades.Controllers
{
    public class HomeController : Controller
    {
        ModelManage Mmodel;

        public ActionResult Index()
        {
            Mmodel = new ModelManage();
            IAsset asset1 = new Equity("gougeul");
            Mmodel.Assets.Add_Asset(asset1, 4);
            IAsset asset2 = new Equity("apeul");
            Mmodel.Assets.Add_Asset(asset2, 3);
            IAsset asset3 = new Equity("mikrosofte");
            Mmodel.Assets.Add_Asset(asset3, 6);
            IAsset asset4 = new Equity("startupdekevintheoetbaptiste");
            Mmodel.Assets.Add_Asset(asset4, 3);

            return View(Mmodel);
        }

        private bool Yahoo_Finance_Parsing()
        {
            return false;
        }

        public bool Buy_Asset()
        {
            return false;
        }
    
    }
}
