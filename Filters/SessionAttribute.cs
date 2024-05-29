using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LibraryNET.Filters
{
    public class SessionAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Verificar si hay una sesión iniciada
            if (filterContext.HttpContext.Session["UserId"] == null)
            {
                // No hay una sesión iniciada, redirigir a la página de inicio de sesión
                filterContext.Result = new RedirectResult("~/Account/Login");
                return;
            }

            // Hay una sesión iniciada, permitir el acceso a la acción
            base.OnActionExecuting(filterContext);
        }
    }
}