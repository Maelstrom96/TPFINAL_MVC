using Oracle.ManagedDataAccess.Client;
using OrDragon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OrDragon.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            OracleConnection conn = Connection.GetConnection();
            conn.Open();
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}