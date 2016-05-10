using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Web.Helpers;
using OrDragon.Models.Game;
using System.Web.Script.Serialization;
using System.Web.Mvc;

namespace OrDragon.Controllers
{
    public class MapHub : Hub
    {
        static int userId;


        public void CreateUser()
        {
            userId++;
            Clients.All.createUser(userId);
        }

        public void SendEntities(List<Entity> entities)
        {
            ActionExecutingContext filterContext = new ActionExecutingContext();
            filterContext.Result = new JsonResult
            {
                Data = entities,
                ContentEncoding = System.Text.Encoding.UTF8,
                ContentType = "application/json",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };

            Clients.All.SendEntities(filterContext.Result);
        }
    }
}