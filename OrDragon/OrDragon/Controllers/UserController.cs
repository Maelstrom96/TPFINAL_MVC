using OrDragon.Exceptions;
using OrDragon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OrDragon.Controllers
{
    public class UserController : Controller
    {
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(UserViewModel userViewModel)
        {
            User user = new Models.User(userViewModel);
            try
            {
                user = Users.Login(user);
                Session["user"] = user;
            }
            catch(InvalidLoginException ix)
            {
                
            }
            return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(UserViewModel userViewModel)
        {
            if (ModelState.IsValid)
            {
                User user = new User(userViewModel);
                try
                {
                    Users.Register(user);
                }
                catch(UsernameAlreadyUsedException ex)
                {
                    ModelState.AddModelError("UserName", "Ce nom d'usager existe déjà!");
                    return View();
                }

                var modal = new Modal();
                modal.Title = "Votre enregistrement est complété";
                modal.Message = "Vous pouvez à présent vous connecter à partir du boutton Login.";
                TempData["Modal"] = modal; // Show popup

                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        public ActionResult LogOff()
        {
            if ((User)Session["User"] != null) // If current session is logged in and as a user object
            {
                Session.Clear();
                Session.Abandon(); // Delete current session
            }
            return RedirectToAction("Index", "Home"); // Send user to index
        }
	}
}