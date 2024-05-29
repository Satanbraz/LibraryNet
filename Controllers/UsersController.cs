using System.Collections.Generic;
using System.Web.Mvc;
using DTO;
using LibraryNET.Filters;
using LibraryNET.Implementation;

namespace LibraryNET.Controllers
{
    [Session]
    [RolPermission]
    public class UsersController : Controller
    {
        #region Propierties

        UserManager _userManager = new UserManager();

        #endregion

        #region UserMethods

        // GET: Users
        public ActionResult Index()
        {
            return View();
        }

        // Metodo para ver el registro de un usuario
        public ActionResult Details(Users user)
        {
            var oDataUser = _userManager.Read(user.Id);

            return Json(oDataUser, JsonRequestBehavior.AllowGet);
        }

        // Metodo para generar un nuevo usuario
        public string Create(Users user)
        {
            var id = _userManager.Create(user);

            if (id > 0)
            {

                return "Ok";
            }
            else
            {
                return null;
            }
        }

        // Metodo para actualizar el registro de un usuario
        public string Edit(Users user)
        {
            var id = _userManager.Update(user);

            if (id == 1)
            {

                return "Ok";
            }
            else
            {
                return null;
            };
        }

        // Metodo para eliminar un registro de usuario
        public string Delete(int userId)
        {
            var id = _userManager.Delete(userId);

            if (id == 1)
            {

                return "Ok";
            }
            else
            {
                return null;
            }
        }

        // Metodo para mostrar la lista total de usuarios
        public ActionResult UserList()
        {
            var oListUsers = _userManager.List();

            ViewBag.UserList = oListUsers;
            
            return View(oListUsers);
        }

        // Metodo para listar los roles de usuario en un JSON
        public ActionResult RolListJson()
        {
            var oListUserRol = _userManager.RolList();

            return Json(oListUserRol, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}