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
