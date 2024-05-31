using DTO;
using LibraryNET.Filters;
using LibraryNET.Implementation.AdminManagers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LibraryNET.Controllers
{
    public class BorrowController : Controller
    {
        #region Propierties

        BorrowManager _borrowManager = new BorrowManager();
        GUID _guidManager = new GUID();

        #endregion


        // GET: Borrow
        public ActionResult Index()
        {
            return View();
        }

        // GET: Borrow/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Borrow/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Borrow/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Borrow/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Borrow/Edit/5
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

        // GET: Borrow/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Borrow/Delete/5
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

        // GET: BorrowList
        public ActionResult BorrowList()
        {
            var oListBorrows = _borrowManager.GetBorrows(Convert.ToInt32(Session["UserId"]));

            ViewBag.BorrowList = oListBorrows;

            return View(oListBorrows);
        }

        public string ProcessBorrow(Borrows borrowModel, List<BorrowDetail> borrowDetailModel)
        {
            DataTable borrowDetail = new DataTable();
            borrowDetail.Columns.Add("IdProduct", typeof(int));
            borrowDetail.Columns.Add("QProducto", typeof(int));
            borrowDetail.Columns.Add("BorrowDate", typeof(DateTime));
            borrowDetail.Columns.Add("BorrowReturnDate", typeof(DateTime));

            foreach (var item in borrowDetailModel)
            {
                borrowDetail.Rows.Add(new Object[]
                {
                    item.BorrowProductId,
                    item.BorrowBookQ,
                    item.BorrowDate,
                    item.BorrowReturnDate
                });
            }

            borrowModel.IdTransaction = "BRID" +  _guidManager.GenerateTransactionId();
            borrowModel.ProductsQ = borrowDetailModel.Count();

            int result = _borrowManager.InsertBorrowRequest(borrowModel, borrowDetail);

            if (result == 1)
            {
                return "Ok";
            }
            else
            {
                return null;
            }
        }

    }
}
