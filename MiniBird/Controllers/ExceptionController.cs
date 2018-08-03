using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MiniBird.Controllers
{
    public class ExceptionController : Controller
    {
        // GET: Exception
        public ActionResult Error(string message)
        {
            if(Request.IsAjaxRequest())
            {
                return PartialView("_AjaxErrorView");
            }

            return Content(message);
        }

        public ActionResult SQLError(string message)
        {
            return Content(message);
        }
    }
}