using OrDragon.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OrDragon.Controllers
{
    public class StatsController : Controller
    {
        // GET: State
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetStats()
        {
            ActionExecutingContext filterContext = new ActionExecutingContext();
            LoginStatus status = new LoginStatus();

            PlayersStats ps = new PlayersStats();
            ps.list = PlayersStats.Initialize();

            filterContext.Result = new JsonResult
            {
                Data = ps.list,
                ContentEncoding = System.Text.Encoding.UTF8,
                ContentType = "application/json",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };

            return filterContext.Result;
        }
    }
}