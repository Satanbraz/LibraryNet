using DTO;
using LibraryNET.Filters;
using LibraryNET.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LibraryNET.Controllers
{
    [Session]
    public class GenderController : Controller
    {
        #region Propierties

        GenderManager _genderManager = new GenderManager();

        #endregion

        #region GenderMethods

        // GET: Gender
        [RolPermission]
        public ActionResult Index()
        {
            return View();
        }

        // Metodo para ver el registro de un genero
        [RolPermission]
        public ActionResult Details(Gender gender)
        {
            var oDataGendert = _genderManager.Read(gender.Id);

            return Json(oDataGendert, JsonRequestBehavior.AllowGet);
        }

        // GET: Gender/Create
        [RolPermission]
        public ActionResult Create()
        {
            return View();
        }

        // Metodo para generar un nuevo genero
        [RolPermission]
        [HttpPost]
        public string Create(Gender gender)
        {
            var id = _genderManager.Create(gender);

            if (id > 0)
            {

                return "Ok";
            }
            else
            {
                return null;
            }
        }

        // GET: Gender/Edit/5
        [RolPermission]
        public ActionResult Edit(int id)
        {
            return View();
        }

        // Metodo para actualizar el registro de un genero
        [RolPermission]
        [HttpPost]
        public string Edit(Gender gender)
        {
            var id = _genderManager.Update(gender);

            if (id == 1)
            {

                return "Ok";
            }
            else
            {
                return null;
            }
        }

        // Metodo para eliminar un registro de genero
        [RolPermission]
        [HttpPost]
        public string Delete(int genderId)
        {
            var id = _genderManager.Delete(genderId);

            if (id == 1)
            {

                return "Ok";
            }
            else
            {
                return null;
            }
        }

        // Metodo para mostrar la lista total de generos
        [RolPermission]
        public ActionResult GenderList()
        {
            var oListGenders = _genderManager.List();

            ViewBag.GenderList = oListGenders;

            return View(oListGenders);
        }

        // Metodo para listar los generos en un JSON
        public ActionResult GenderListJson()
        {
            var oListGenders = _genderManager.List();

            return Json(oListGenders, JsonRequestBehavior.AllowGet);
        }
        
        #endregion
    }
}
