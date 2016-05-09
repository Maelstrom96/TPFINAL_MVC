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
            if ((User)Session["User"] != null) return View();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(UserViewModel userViewModel)
        {
            if ((User)Session["User"] != null)
            {
                User user = new Models.User(userViewModel);
                try
                {
                    user = Users.Login(user);
                    Session["user"] = user;
                }
                catch (InvalidLoginException ix)
                {
                    var modal = new Modal();
                    modal.Title = "Login failed";
                    modal.Message = "Le nom d'usagée ou le mot de passe est invalide.";
                    TempData["Modal"] = modal; // Show popup
                }
            }
            return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
        }

        public ActionResult ValidateUser(string username, string password)
        {
            LoginStatus status = new LoginStatus();
            ActionExecutingContext filterContext = new ActionExecutingContext();

            if ((User)Session["User"] != null)
            {
                status.Success = true;
                status.Message = "Une session est déja active.";

                filterContext.Result = new JsonResult
                {
                    Data = status,
                    ContentEncoding = System.Text.Encoding.UTF8,
                    ContentType = "application/json",
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };

                return filterContext.Result;
            }

            User user = new User();
            user.Username = username;
            user.Password = Security.HashSHA1(password);

            try
            {
                user = Users.Login(user);
                Session["user"] = user; // register user to session

                status.Success = true;
                status.Message = "Login attempt successful!";
            }
            catch (InvalidLoginException ix)
            {
                status.Success = false;
                status.Message = "Invalid UserID or Password!";
                status.TargetURL = null;
            }

            filterContext.Result = new JsonResult
            {
                Data = status,
                ContentEncoding = System.Text.Encoding.UTF8,
                ContentType = "application/json",
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };

            return filterContext.Result;
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