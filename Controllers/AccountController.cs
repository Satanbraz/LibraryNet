using LibraryNET.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LibraryNET.Controllers
{
    public class AccountController : Controller
    {
        #region Propierties

        LoginManager _loginManager = new LoginManager();

        #endregion

        #region LoginMethods

        public ActionResult Login()
        {
            return View();
        }


        // Método para iniciar sesión
        [HttpPost]
        public ActionResult Login(string User, string Password)
        {
            var userLogin = _loginManager.Read(User,Password);

            if (userLogin == null)
            {
                TempData["ErrorMessage"] = "Usuario o contraseña incorrectos.";
                return View();
            }
            else
            {
                Session["UserId"] = userLogin.UserRolId;
                Session["UserName"] = userLogin.UserName;
                Session["UserLName"] = userLogin.UserLastName;
                Session["UserRole"] = userLogin.UserRolName;
                Session["UserPhone"] = userLogin.UserPhone;
                Session["UserDir"] = userLogin.UserDir;
                Session["LoginTime"] = DateTime.Now;
            }
            return RedirectToAction("Index", "Home");
        }

        // Método para cerrar sesión
        public void Logout()
        {
            // Limpiar la sesión
            Session.Clear();
        }


        // Método para calcular tiempo restante de sesión
        public ActionResult RemainingTime()
        {

            if (Session["UserId"] != null && Session["LoginTime"] != null)
            {
                int sessionDurationMinutes = Session.Timeout;
                DateTime loginTime = (DateTime)Session["LoginTime"];
                TimeSpan elapsedTime = DateTime.Now - loginTime;
                TimeSpan remainingTime = TimeSpan.FromMinutes(sessionDurationMinutes) - elapsedTime;

                return Json(remainingTime.ToString("hh\\:mm\\:ss"), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("SessionExpired", JsonRequestBehavior.AllowGet);
            }
        }

        #endregion
    }
}
