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
    public class BuySellController : Controller
    {
        #region Propierties

        GUID _guidManager = new GUID();
        BuySellManager _buySellManager = new BuySellManager();

        #endregion

        // GET: BuySell
        public ActionResult Index()
        {
            return View();
        }

        // GET: BuySell/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: BuySell/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: BuySell/Create
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

        // GET: BuySell/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: BuySell/Edit/5
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

        // GET: BuySell/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: BuySell/Delete/5
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

        #region SellMethods

        #endregion

        #region BuyMethods
        public string ProcessPayment(Buy buyModel, List<BuyDetail> buyDetailModel)
        {
            int buySubtotal = 0;
            DataTable buyDetail = new DataTable();
            buyDetail.Columns.Add("IdProduct",typeof(int));
            buyDetail.Columns.Add("QProducto", typeof(int));
            buyDetail.Columns.Add("Total", typeof(int));

            foreach (var item in buyDetailModel)
            {
                int subtotal = item.BookPrice * item.BuyBookQ;

                buySubtotal += subtotal;

                buyDetail.Rows.Add(new Object[]
                {
                    item.BuyProductId,
                    item.BuyBookQ,
                    subtotal
                });
            }

            buyModel.TotalBruto = buySubtotal;
            buyModel.IVA = (int)(buySubtotal * 0.19);
            buyModel.TotalAmount = buyModel.TotalBruto + buyModel.IVA;
            buyModel.BuyDate = DateTime.Now;
            buyModel.IdTransaction = _guidManager.GenerateTransactionId();
            buyModel.ProductsQ = buyDetailModel.Count();

            int result = _buySellManager.InsertBuy(buyModel, buyDetail);

            if (result == 1)
            {
                return "Ok";
            }
            else
            {
                return null;
            }
        }

        #endregion
    }
}
