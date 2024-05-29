using DTO;
using LibraryNET.Filters;
using LibraryNET.Implementation;
using LibraryNET.Implementation.AdminManagers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LibraryNET.Controllers
{
    [Session]
    public class BooksController : Controller
    {
       
        #region Propierties

        BooksManager _booksManager = new BooksManager();

        #endregion

        #region BooksMethods

        // GET: Books
        public ActionResult Index()
        {
            return View();
        }

        // GET: Books/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Books/Create
        public ActionResult Create()
        {
            return View();
        }

        // Metod para agregar un nuevo libro
        [HttpPost]
        public string Create(Books books)
        {

            var id = _booksManager.Create(books);

            if (id > 0)
            {

                return "Ok";
            }
            else 
            {
                return null;
            }
        }

        // GET: Books/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Books/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Books/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Books/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // Metodo para mostrar lista total de libros
        public ActionResult BooksList()
        {
            var oListBooks = _booksManager.List();

            ViewBag.BooksList = oListBooks;

            return View(oListBooks);
        }

        // Metodo para mostrar lista de libros por filtro
        public ActionResult BooksListFiltered(int BookGenderId, string Busqueda)
        {
            var oListBooksFiltered = _booksManager.ListFiltered(BookGenderId, Busqueda);

            ViewBag.BooksList = oListBooksFiltered;

            return View("BooksList",oListBooksFiltered);
        }

        // Metodo para cargar/cambiar imagen a libro
        [HttpPost]
        public ActionResult UploadImage(int bookId, HttpPostedFileBase image)
        {
            if (image != null && image.ContentLength > 0)
            {
                try
                {
                    // Leer los datos de la imagen y convertirlos en bytes
                    byte[] imageData = null;
                    using (var binaryReader = new BinaryReader(image.InputStream))
                    {
                        imageData = binaryReader.ReadBytes(image.ContentLength);
                    }

                    // Guardar los datos de la imagen en la base de datos (aquí asumimos que tienes un método en tu clase de manejo de datos para hacer esto)
                    _booksManager.SaveBookImage(bookId, imageData);

                    return Json(new { success = true });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, error = ex.Message });
                }
            }
            else
            {
                return Json(new { success = false, error = "No se ha seleccionado ninguna imagen." });
            }
        }

        #endregion
    }
}
