using MiniBird.DTO;
using Service_Layer;
using Domain_Layer;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static MiniBird.Filters.SessionFilters;
using Domain_Layer.DTO;

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

                    return RedirectToAction("Timeline", "Account");
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
        
        public ActionResult ProfileScreen(string v)
        {
            var model = Account.ProfileScreenCollectionDataSL(ActiveSession.GetPersonID(), v);
            ViewBag.Tab = v;
            return View(model);
        }

        public ActionResult Timeline()
        {
            try
            {
                var model = Account.TimelineCollectionDataSL(ActiveSession.GetPersonID());
                return View(model);
            }
            catch(Exception ex)
            {
                return ProcessError(ex);
            }            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewPost(NewPostDTO model)
        {
            if (!ModelState.IsValid)
                return Content("Fallaron las validaciones.");

            if (Account.CreateNewPostSL(model.Comment, model.GifImage, model.VideoFile, model.ImagesUploaded, ActiveSession.GetPersonID(), model.InReplyTo))
            {
                TempData["message"] = "El post se ha publicado.";
                return RedirectToAction("Timeline", "Account");
            }

            return RedirectToAction("Index", "Home");
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

        [HttpPost]
        public string ChangeHeader()
        {
            if (System.Web.HttpContext.Current.Request.Files.AllKeys.Any())
            {
                var headerFile = System.Web.HttpContext.Current.Request.Files["ImageFile"];
                return Account.ChangeHeaderSL(headerFile, ActiveSession.GetPersonID());
            }

            return "";
        }

        [HttpPost]
        public string ChangeAvatar()
        {
            if (System.Web.HttpContext.Current.Request.Files.AllKeys.Any())
            {
                var avatarFile = System.Web.HttpContext.Current.Request.Files["ImageFile"];
                return Account.ChangeAvatarSL(avatarFile, ActiveSession.GetPersonID());
            }

            return "";
        }

        [HttpGet]
        public PartialViewResult EditDetailsForm()
        {
            var model = Account.ChangeProfileDetailsSL(ActiveSession.GetPersonID());
            return PartialView("_EditProfileDetails", model);
        }

        [HttpPost]
        public JsonResult EditDetailsForm(ProfileDetailsDTO model)
        {
            Account.ChangeProfileDetailsSL(model, ActiveSession.GetPersonID());
            return Json(new { personalDescription = model.PersonalDescription, websiteURL = model.WebSiteURL, birthDate = model.Birthdate });
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