using OrDragon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OrDragon.Controllers
{
    public class StateController : Controller
    {
        // GET: State
        public ActionResult Index()
        {
            PlayerState p = new PlayerState();
            p = p.Initialize();
            PlayersStates ps = new PlayersStates();
            ps.list = ps.Initialize();
            return View();
        }
    }
}