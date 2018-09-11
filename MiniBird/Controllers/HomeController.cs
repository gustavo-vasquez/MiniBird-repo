using Domain_Layer;
using MiniBird.DTO;
using Service_Layer;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MiniBird.Controllers
{
    public class HomeController : Controller
    {
        static AccountSL Account = new AccountSL();        
                
        public ActionResult Welcome()
        {
            if (ActiveSession.IsAuthenticated)
                return RedirectToAction("Timeline", "Account");
            else
                return View();

            
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(SignInDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View("Welcome", model);

                if (Account.RegisterSL(model.Register.UserName, model.Register.Email, model.Register.Password))
                {
                    Session["MiniBirdAccount"] = Account.CreateSessionSL(model.Register.Email);
                    return RedirectToAction("Timeline", "Account");
                }
                else
                {
                    ViewBag.Message = "El usuario/correo ya está registrado.";
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                return ProcessError(ex);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(SignInDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View("Welcome", model);

                if (Account.LoginSL(model.Login.Email, model.Login.Password))
                {
                    Session["MiniBirdAccount"] = Account.CreateSessionSL(model.Login.Email);

                    if (model.Login.RememberMe)
                        LoginCookie(model.Login.Email);

                    return RedirectToAction("Timeline", "Account");
                }
                else
                {
                    ViewBag.Message = "Datos incorrectos. Vuelva a iniciar sesión.";
                    ViewBag.Login = "activate";
                    return View("Welcome", model);
                }
            }
            catch (Exception ex)
            {
                return ProcessError(ex);
            }
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
        
        public PartialViewResult Search()
        {
            return PartialView("_Search");
        }


        #region TAREAS AUXILIARES

        public void LoginCookie(string email)
        {
            try
            {
                HttpCookie cookie = new HttpCookie("MBLC");
                cookie.Domain = "localhost";
                cookie.Expires = DateTime.Now.AddDays(30);
                cookie.Path = "/";
                cookie.Secure = false;
                cookie.Value = Account.EncryptCookieValueSL(email);
                Response.Cookies.Add(cookie);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion


        #region MANEJAR EXCEPCIONES

        public ActionResult ProcessError(Exception ex)
        {
            return RedirectToAction("Error", "Exception", new
            {
                message = ex.InnerException is SqlException ? ex.InnerException.Message : ex.Message
            });
        }

        #endregion
    }
}