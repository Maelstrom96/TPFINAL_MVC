using OrDragon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OrDragon.Controllers
{
    public class MapController : Controller
    {
        // GET: Map
        public ActionResult GetNoeuds()
        {
            Map map = (Map)HttpRuntime.Cache["map"];
            ActionExecutingContext filterContext = new ActionExecutingContext();

            filterContext.Result = new JsonResult
            {
                Data = map.noeuds,
                ContentEncoding = System.Text.Encoding.UTF8,
                ContentType = "application/json",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };

            return filterContext.Result;
        }

        public ActionResult GetChemins()
        {
            Map map = (Map)HttpRuntime.Cache["map"];
            ActionExecutingContext filterContext = new ActionExecutingContext();

            filterContext.Result = new JsonResult
            {
                Data = map.chemins,
                ContentEncoding = System.Text.Encoding.UTF8,
                ContentType = "application/json",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };

            return filterContext.Result;
        }
    }
}