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
                //Users users = (Users)HttpRuntime.Cache["Users"];
                //User foundUser = users.GetUserByName(userViewModel.Username);
                //if (foundUser != null)
                //    ModelState.AddModelError("UserName", "Ce nom d'usager existe déjà!");
                //else
                //{
                //    User newUser = new User(userViewModel);
                //    users.Add(newUser);

                //    UserViewModel usv = new UserViewModel();
                //    usv.Password = newUser.Password;
                //    usv.ConfirmPassword = newUser.Password;
                //    usv.Username = newUser.Username;
                //    Login(usv);

                //    return RedirectToAction("Index", "Home"); // Atteindra jamais
                //}
            }
            return View();
        }

        public ActionResult LogOff()
        {
            if ((User)Session["User"] != null)
            {
                Session.Clear();
                Session.Abandon(); // Delete current session
            }
            return RedirectToAction("Index", "Home"); // Send user to index
        }
	}
}