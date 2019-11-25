using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebAppl.CustomAuthentication;
using System.Security;
using Newtonsoft.Json;
using WebAppl.Models;

namespace WebAppl.Controllers
{
    public class AccountController : Controller
    {
        /*
        // GET: Account
        public ActionResult Index()
        {
            return View();
        }
        */
        [HttpGet]
        public ActionResult Login(string ReturnUrl = "")
        {
            if (User.Identity.IsAuthenticated)
            {
                return LogOut();
            }
            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }

        [HttpPost]
        public ActionResult Login(AccountModel LoginView, string ReturnUrl = "")
        {
            if (ModelState.IsValid)
            {
                if (Membership.ValidateUser(LoginView.UserName, LoginView.Password))
                {
                    var user = (CustomMembershipUser)Membership.GetUser(LoginView.UserName, false);
                    if (user != null)
                    {
                       
                        FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket
                            (
                            1, LoginView.UserName, DateTime.Now, DateTime.Now.AddMinutes(15), false, user.UserId.ToString()
                            );

                        string enTicket = FormsAuthentication.Encrypt(authTicket);
                        HttpCookie faCookie = new HttpCookie("AuthCookie", enTicket);
                        Response.Cookies.Add(faCookie);
                    }

                    if (Url.IsLocalUrl(ReturnUrl))
                    {
                        return Redirect(ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index","Home");
                    }
                }
            ModelState.AddModelError("", "Bitte überprüfen Sie Ihre Eingaben.");
            }
            return View(LoginView);
        }



        public ActionResult LogOut()
        {
            HttpCookie cookie = new HttpCookie("AuthCookie", "");
            cookie.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie);

            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account", null);
        }

    }
}