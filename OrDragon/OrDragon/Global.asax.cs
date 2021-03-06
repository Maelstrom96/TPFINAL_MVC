﻿using OrDragon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace OrDragon
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            Questions questions = new Questions();
            questions.Import();
            HttpRuntime.Cache["questions"] = questions;

            Map map = new Map();
            map.GetFromServer();
            map.StartListener();
            HttpRuntime.Cache["map"] = map;
        }

        protected void Session_Start()
        {
            Questions q = (Questions)HttpRuntime.Cache["questions"];
            q.Import();
        }
    }
}
