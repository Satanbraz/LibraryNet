using System.Web.Mvc;
using System.Web.Routing;

namespace LibraryNET.Filters
{
    public class RolPermission : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Verificar si el usuario tiene el rol adecuado
            if (filterContext.HttpContext.Session["UserRole"] != null && (string)filterContext.HttpContext.Session["UserRole"] == "Admin")
            {
                // El usuario es administrador, permitir el acceso
                base.OnActionExecuting(filterContext);
            }
            else
            {
                // El usuario no tiene el rol adecuado, redirigir a una página de error o realizar otra acción
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Error", action = "AccesoNoAutorizado" }));
            }
        }


    }
}