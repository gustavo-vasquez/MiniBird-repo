using MiniBird.DTO;
using Service_Layer;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static MiniBird.Filters.SessionFilters;

namespace MiniBird.Controllers
{    
    [Authenticated]
    public class AccountController : Controller
    {
        static AccountSL Account = new AccountSL();

        // GET: User
        //[WithAccount(false)]
        [Authenticated(false)]
        public ActionResult Register()
        {
            return View();
        }

        [Authenticated(false)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(SignInDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(model);                

                if(Account.RegisterSL(model.Register.UserName, model.Register.Email, model.Register.Password))
                {
                    Session["MiniBirdAccount"] = Account.CreateSessionSL(model.Register.Email);
                    return RedirectToAction("Index", "Home");
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

        [Authenticated(false)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(SignInDTO model)
        {
            try
            {
                if (!ModelState.IsValid)                
                    return View("Register", model);

                if(Account.LoginSL(model.Login.Email, model.Login.Password))
                {
                    Session["MiniBirdAccount"] = Account.CreateSessionSL(model.Login.Email);

                    if(model.Login.RememberMe)                    
                        LoginCookie(model.Login.Email);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.Message = "Datos incorrectos. Vuelva a iniciar sesión.";
                    ViewBag.Login = "activate";
                    return View("Register", model);
                }
            }
            catch (Exception ex)
            {
                return ProcessError(ex);
            }
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            Session.Abandon();
            Domain_Layer.ActiveSession.Clear();

            if (Request.Cookies.AllKeys.Contains("MBLC"))
            {
                HttpCookie cookie = Request.Cookies["MBLC"];
                cookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(cookie);
            }

            return RedirectToAction("Index", "Home");
        }

        [Authenticated(false)]
        public ActionResult ProfileScreen(string v)
        {
            ViewBag.Tab = v;
            return View();
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


        #region PETICIONES AJAX

        [Authenticated(false)]
        [HttpGet]
        public JsonResult CheckUserName(string username)
        {
            return Json(new { userExists = Account.UserNameExistsSL(username) }, JsonRequestBehavior.AllowGet);
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