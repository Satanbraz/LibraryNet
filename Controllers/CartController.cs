using DTO;
using LibraryNET.Filters;
using LibraryNET.Implementation.AdminManagers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LibraryNET.Controllers
{
    [Session]
    public class CartController : Controller
    {
        #region Propierties

        CartManager _cartManager = new CartManager();

        #endregion

        #region CartMethods

        // GET: Cart
        public ActionResult Index()
        {
            return View();
        }

        // GET: Cart/Details/5
        public ActionResult CartDetail()
        {
            var oCartList = _cartManager.GetCartDetail(Convert.ToInt32(Session["UserId"]));

            ViewBag.BooksList = oCartList;

            return View("CartDetail", oCartList);
        }

        // POST: Cart/Create
        [HttpPost]
        public string AddToCart(Cart cart)
        {
            var id = _cartManager.AddBookToCart(cart);

            if (id > 0)
            {

                return "Ok";
            }
            else
            {
                return null;
            }
        }

        // GET: Cart/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Cart/Edit/5
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

        // GET: Cart/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Cart/Delete/5
        [HttpPost]
        public string DelFromCart(int idCart, int idBook)
        {
            var result = _cartManager.RemoveBookFromCart(idCart, idBook);

            if (result == 1)
            {
                return "Ok";
            }
            else
            {
                return null;
            }
        }

        // GET: Cart
        public int FindCart()
        {
            var result = _cartManager.VarifyCart(Convert.ToInt32(Session["UserId"]));

            if (result == 0)
            {
                return 0;
            }
            else
            {
                return result;
            }

            
        }

        public ActionResult QCart()
        {
            var cantidad = _cartManager.GetCartDetail(Convert.ToInt32(Session["UserId"])).Count();

            return Json(new { cantidad = cantidad }, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}
