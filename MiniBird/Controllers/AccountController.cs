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
        //[Authenticated(false)]
        //public ActionResult Register()
        //{
        //    return View();
        //}

        //[Authenticated(false)]
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Register(SignInDTO model)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //            return View(model);                

        //        if(Account.RegisterSL(model.Register.UserName, model.Register.Email, model.Register.Password))
        //        {
        //            Session["MiniBirdAccount"] = Account.CreateSessionSL(model.Register.Email);
        //            return RedirectToAction("Index", "Home");
        //        }
        //        else
        //        {
        //            ViewBag.Message = "El usuario/correo ya está registrado.";
        //            return View(model);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return ProcessError(ex);
        //    }
        //}

        //[Authenticated(false)]
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Login(SignInDTO model)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid)                
        //            return View("Welcome", model);

        //        if(Account.LoginSL(model.Login.Email, model.Login.Password))
        //        {
        //            Session["MiniBirdAccount"] = Account.CreateSessionSL(model.Login.Email);

        //            if(model.Login.RememberMe)                    
        //                LoginCookie(model.Login.Email);

        //            return RedirectToAction("Timeline", "Account");
        //        }
        //        else
        //        {
        //            ViewBag.Message = "Datos incorrectos. Vuelva a iniciar sesión.";
        //            ViewBag.Login = "activate";
        //            return View("Register", model);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return ProcessError(ex);
        //    }
        //}
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            Session.Abandon();
            ActiveSession.Clear();

            if (Request.Cookies.AllKeys.Contains("MBLC"))
            {
                HttpCookie cookie = Request.Cookies["MBLC"];
                cookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(cookie);
            }

            return RedirectToAction("Welcome", "Home");
        }
        
        public ActionResult ProfileScreen(int id, string v)
        {
            try
            {
                var model = Account.ProfileScreenCollectionDataSL(id, v);
                ViewBag.Tab = v;

                return View(model);
            }
            catch(Exception ex)
            {
                return ProcessError(ex);
            }
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

            if (Account.CreateNewPostSL(model, ActiveSession.GetPersonID(), Server))
            {
                TempData["message"] = "El post se ha publicado.";
                return RedirectToAction("Timeline", "Account");
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewList(ListDTO model)
        {
            if(!ModelState.IsValid)            
                return View(model);

            Account.NewListSL(model, ActiveSession.GetPersonID());

            return RedirectToAction("ProfileScreen", "Account", new { v = "lists" });
        }

        public ActionResult ListScreen(int id)
        {
            var model = Account.ListScreenCollectionDataSL(id, ActiveSession.GetPersonID());
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditList(ListDTO model, string RedirectUrl)
        {
            Account.EditListSL(model);
            return Redirect(RedirectUrl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveList(ListScreenDTO model)
        {
            Account.RemoveListSL(model.CurrentListSection.MyListID, ActiveSession.GetPersonID());
            return RedirectToAction("ProfileScreen", "Account", new { v = "lists" });
        }

        public ActionResult ViewPost(int postID)
        {
            try
            {
                if (Request.IsAjaxRequest())
                {
                    var model = Account.ViewPostAjaxCollectionDataSL(postID);                    
                    return PartialView("_ViewPost", model);
                }
                else
                {
                    var model = Account.ViewPostCollectionDataSL(postID);                    
                    return View(model);
                }
            }
            catch(Exception ex)
            {
                return ProcessError(ex);
            }
        }        



        #region TAREAS AUXILIARES        

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
            if (!ModelState.IsValid)
                return Json(new { message = "Fallaron las validaciones" });

            ProfileDetailsDTO newDetails = Account.ChangeProfileDetailsSL(model, ActiveSession.GetPersonID());
            return Json(new { personalDescription = newDetails.PersonalDescription, websiteURL = newDetails.WebSiteURL, birthDate = newDetails.Birthdate });
        }

        public PartialViewResult ImagePreview()
        {
            return PartialView("_ImagePreview");
        }

        [HttpPost]
        public PartialViewResult SendRepost(int postID)
        {
            Account.SendRepostSL(postID, ActiveSession.GetPersonID());
            return PartialView("_InteractButtons", Account.GetInteractsCountSL(postID));
        }

        [HttpPost]
        public PartialViewResult GiveALike(int postID)
        {
            Account.GiveALikeSL(postID, ActiveSession.GetPersonID());
            return PartialView("_InteractButtons", Account.GetInteractsCountSL(postID));
        }

        [HttpGet]
        public PartialViewResult DrawPublication(string call, string updateTarget)
        {
            switch(call)
            {
                case "post":
                    return PartialView("_NewPost");
                case "reply":
                    TempData["UpdateDiv"] = updateTarget;
                    return PartialView("_NewReply");
                default:
                    return null;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewReply(NewPostDTO model)
        {
            if (!ModelState.IsValid)
                return Content("Fallaron las validaciones.");

            if (Account.CreateNewReplySL(model, ActiveSession.GetPersonID(), Server))
            {
                var newModel = Account.ViewPostCollectionDataSL(Convert.ToInt32(model.InReplyTo));
                return PartialView("_RepliesToPost", newModel.RepliesToPost);
            }

            return RedirectToAction("Timeline", "Account");
        }

        public JsonResult FollowUser(int follow)
        {
            if (Account.FollowUserSL(ActiveSession.GetPersonID(), follow))
                return Json(new { buttonText = "Dejar de seguir", className = "btn-danger" }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { buttonText = "Seguir", className = "btn-success" }, JsonRequestBehavior.AllowGet);
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