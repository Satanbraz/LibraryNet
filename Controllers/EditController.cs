using DTO;
using LibraryNET.Filters;
using LibraryNET.Implementation;
using LibraryNET.Implementation.AdminManagers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LibraryNET.Controllers
{
    [Session]
    public class EditController : Controller
    {
        #region Propierties

        EditManager _editManager = new EditManager();

        #endregion

        #region EditMethods

        // GET: Edit
        [RolPermission]
        public ActionResult Index()
        {
            return View();
        }

        // Metodo para ver el registro de una editorial
        [RolPermission]
        public ActionResult Details(int id)
        {
            var oDataEdit = _editManager.Read(id);

            return Json(oDataEdit, JsonRequestBehavior.AllowGet);
        }

        // GET: Edit/Create
        [RolPermission]
        public ActionResult Create()
        {
            return View();
        }

        // Metodo para generar un nuevo editorial
        [RolPermission]
        [HttpPost]
        public string Create(Edit edit)
        {
            var id = _editManager.Create(edit);

            if (id > 0)
            {

                return "Ok";
            }
            else
            {
                return null;
            }
        }

        // GET: Edit/Edit/5
        //public ActionResult Edit(int id)
        //{
        //    return View();
        //}

        // Metodo para actualizar el registro de una editorial
        [RolPermission]
        [HttpPost]
        public string Edit(Edit edit)
        {
            var id = _editManager.Update(edit);

            if (id == 1)
            {

                return "Ok";
            }
            else
            {
                return null;
            }
        }

        // Metodo para eliminar un registro de editorial
        [RolPermission]
        [HttpPost]
        public string Delete(int editId)
        {
            var id = _editManager.Delete(editId);

            if (id == 1)
            {

                return "Ok";
            }
            else
            {
                return null;
            }
        }

        // Metodo para mostrar la lista total de editoriales
        [RolPermission]
        public ActionResult EditList()
        {
            var oListEdits = _editManager.List();

            ViewBag.EditList = oListEdits;

            return View(oListEdits);
        }

        // Metodo para listar las editoriales en un JSON
        public ActionResult EditListJson()
        {
            var oListEdits = _editManager.List();

            return Json(oListEdits, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}
