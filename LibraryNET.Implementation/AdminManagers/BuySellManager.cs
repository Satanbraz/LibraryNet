﻿using DTO;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls.WebParts;
using System.Security.Cryptography;

namespace LibraryNET.Implementation.AdminManagers
{
    public class BuySellManager
    {
        //Agregar conexion a BD
        DbConnectionProvider dbc = new DbConnectionProvider();

        #region Propierties

        protected string TableName = "Buys";
        protected string Fields = "Id, IdUser, ProductsQ, PhoneUser, DirUser, IdTransaction, BuySubtotal, BuyIVA, BuyTotalAmount, BuyDate";

        #endregion

        #region Public Buy Methods

        public int InsertBuy(Buy buy, DataTable buyDetail)
        {
            int result = 0;

            using (SqlConnection con = dbc.OpenConnection())
            {
                SqlTransaction tran = con.BeginTransaction();

                try
                {
                    // Definir la query para insertar en Compra
                    string query = string.Format("INSERT INTO {0} ({1}) {2}",
                                    TableName,
                                    "IdUser, ProductsQ, PhoneUser, DirUser, IdTransaction, BuySubtotal, BuyIVA, BuyTotalAmount, BuyDate",
                                    "VALUES(@IdUser, @ProductsQ, @PhoneUser, @DirUser, @IdTransaction, @BuySubtotal, @BuyIVA, @BuyTotalAmount, @BuyDate)");

                    // Definir los paramatros de enviar
                    List<SqlParameter> colParameters = new List<SqlParameter>();
                    colParameters.Add(new SqlParameter("IdUser", buy.IdUser));
                    colParameters.Add(new SqlParameter("ProductsQ", buy.ProductsQ));
                    colParameters.Add(new SqlParameter("PhoneUser", buy.PhoneUser));
                    colParameters.Add(new SqlParameter("DirUser", buy.DirUser));
                    colParameters.Add(new SqlParameter("IdTransaction", buy.IdTransaction));
                    colParameters.Add(new SqlParameter("BuySubtotal", buy.TotalBruto));
                    colParameters.Add(new SqlParameter("BuyIVA", buy.IVA));
                    colParameters.Add(new SqlParameter("BuyTotalAmount", buy.TotalAmount));
                    colParameters.Add(new SqlParameter("BuyDate", DateTime.Now));


                    //Ejecutar el comando y obtener el resultado
                    dbc = new DbConnectionProvider();
                    int id = Convert.ToInt32(dbc.ExecuteReturningId(query, "Id", colParameters, TableName));

                    if (id > 0)
                    {
                        try
                        {
                            // Crear lista de BuyDetail
                            List<BuyDetail> buyDetailList = new List<BuyDetail>();

                            // Rellenar lista con registros
                            if ((buyDetail != null) && (buyDetail.Rows.Count > 0))
                            {
                                try
                                {
                                    for (int i = 0; i < buyDetail.Rows.Count; i++)
                                    {
                                        // Transformar las filas del DT en fila de BuyDetail
                                        BuyDetail buyDetailObj = ConvertDtRowToDTO(buyDetail.Rows[i]);
                                        buyDetailObj.BuyId = id;
                                        buyDetailList.Add(buyDetailObj);

                                        // Valida la cantidad comprada de cada libro
                                        if (buyDetailObj.BuyBookQ > 1)
                                        {
                                            try
                                            {
                                                result = updateBookStock(buyDetailObj.BuyBookQ, buyDetailObj.BuyProductId);
                                            }
                                            catch (Exception)
                                            {

                                                throw;
                                            }
                                        }
                                        // Genera un registro en el Detalle de Compra
                                        result = InsertBuyDetail(buyDetailList[i]);
                                    }

                                    if (result == 1)
                                    {
                                        try
                                        {
                                            // Elimina el carrito generado al usuario
                                            result = delCart(buy.IdUser);
                                        }
                                        catch (Exception)
                                        {
                                            return result;
                                            throw;
                                        }
                                    }
                                }
                                catch (Exception)
                                {
                                    return result;
                                    throw;
                                }
                            }
                            else
                            {
                                return result;
                            }
                        }
                        catch (Exception)
                        {
                            return result;
                            throw;
                        }
                    }
                    else
                    {
                        return result;
                    }
                }
                catch (Exception)
                {
                    // Regresa todo en caso de error
                    tran.Rollback();
                    return result;
                    throw;
                }

            }

            return result;
        }

        public IEnumerable<Buy> GetBuys(int idUser)
        {
            string query = string.Format(@"SELECT  
                                        A.Id,
		                                A.IdTransaction,
		                                a.BuySubtotal,
		                                A.BuyIVA,
		                                A.BuyTotalAmount,
                                        A.BuyDate,
		
		                                B.Id as BDetId,
                                        B.BuyId,
		                                B.ProductId,
		                                B.QProduct,
		                                B.ProductAmount,
		
		                                C.BookTitle,
		                                C.BookAuthor,
		                                C.BookImage
		                                FROM {0} as A 
		                                LEFT JOIN BuyDetail as B on A.Id = B.BuyId
		                                LEFT JOIN Books as C on C.Id = B.ProductId 
                                        WHERE A.IdUser = {1}",
                                        TableName,
                                        idUser);

            // Crear conexion y ejecutar la query
            dbc = new DbConnectionProvider();
            DataTable data = dbc.GetDataTable(query);

            // Crear lista de registros
            List<Buy> buyDetailList = new List<Buy>();

            // Rellenar lista con registros
            if ((data != null) && (data.Rows.Count > 0))
            {
                for (int i = 0; i < data.Rows.Count; i++)
                {
                    buyDetailList.Add(ConvertDtRowToDTOGet(data.Rows[i]));
                }
            }

            return buyDetailList;
        }

        #endregion

        #region Protected Methods

        // Convierte el DT en un objeto Clase BuyDetail
        protected BuyDetail ConvertDtRowToDTO(DataRow data)
        {
            BuyDetail buyDetail = new BuyDetail
            {
                BuyProductId = (int)data["IdProduct"],
                BuyBookQ = (int)data["QProducto"],
                BookPrice = (int)data["Total"],
                BuyTotal = (int)data["Total"] * (int)data["QProducto"]
            };

            return buyDetail;
        }

        // Convierte el DT en un objeto Clase Buy
        protected Buy ConvertDtRowToDTOGet(DataRow data)
        {
            Buy buyDetail = new Buy
            {
                Id = (int)data["Id"],
                IdTransaction = data["IdTransaction"].ToString(),
                TotalBruto = (int)data["BuySubtotal"],
                IVA = (int)data["BuyIVA"],
                TotalAmount = (int)data["BuyTotalAmount"],
                BuyDate = DateTime.Parse(data["BuyDate"].ToString()),
                    buyDetail = new BuyDetail()
                    {
                        Id = (int)data["BDetId"],
                        BuyId = (int)data["BuyId"],
                        BuyProductId = (int)data["ProductId"],
                        BuyBookQ = (int)data["QProduct"],
                        BookPrice = (int)data["ProductAmount"]
                    },
                    books = new Books()
                    {
                        Title = data["BookTitle"].ToString(),
                        Author = data["BookAuthor"].ToString(),
                        BookImage = Convert.FromBase64String(data["BookImage"].ToString())
                    }
            };

            return buyDetail;
        }

        // Metodo para agregar un registro en BuyDetail
        protected int InsertBuyDetail(BuyDetail buyDetail)
        {
            int result = 0;

            try
            {
                // Definir la query
                string query = string.Format("INSERT INTO BuyDetail ({0}) {1}",
                                "BuyId, ProductId, QProduct, ProductAmount",
                                "VALUES(@BuyId, @ProductId, @QProduct, @ProductAmount)");

                // Definir los paramatros de enviar

                List<SqlParameter> colParameters = new List<SqlParameter>();
                colParameters.Add(new SqlParameter("BuyId", buyDetail.BuyId));
                colParameters.Add(new SqlParameter("ProductId", buyDetail.BuyProductId));
                colParameters.Add(new SqlParameter("QProduct", buyDetail.BuyBookQ));
                colParameters.Add(new SqlParameter("ProductAmount", buyDetail.BookPrice));


                //Ejecutar el comando y obtener el resultado
                dbc = new DbConnectionProvider();
                int id = Convert.ToInt32(dbc.ExecuteReturningId(query, "Id", colParameters, TableName));
                result = 1;
            }
            catch (Exception)
            {
                return result;
                throw;
            }

            return result;
        }

        // Metodo para eliminar el carrito del usuario
        protected int delCart(int idUser)
        {
            int result = 0;
            try
            {
                //Definir el comando
                string command = string.Format("DELETE FROM Cart WHERE IdUser = {0}",
                                               "@IdUser");

                //Definir los parámetros
                List<SqlParameter> colParameters = new List<SqlParameter>();
                colParameters.Add(new SqlParameter("IdUser", idUser));

                //Ejecutar el comando
                DbConnectionProvider tdBase = new DbConnectionProvider();
                tdBase.Execute(command, colParameters);
                result = 1;
            }
            catch (Exception ex)
            {
                return result;
                throw;
            }
            return result;
        }

        // Metodo para actualizar el stock del libro comprado
        protected int updateBookStock(int bookQ, int bookId)
        {
            int result = 0;

            try
            {
                //Definir el comando
                string command = string.Format("UPDATE Books SET {0} WHERE Id = {1}",
                                               "BookCant = BookCant - @BookCant",
                                               "@Id");

                //Definir los parámetros
                List<SqlParameter> colParameters = new List<SqlParameter>();
                colParameters.Add(new SqlParameter("BookCant", bookQ - 1));
                colParameters.Add(new SqlParameter("Id", bookId));

                //Ejecutar el comando
                DbConnectionProvider tdBase = new DbConnectionProvider();
                tdBase.Execute(command, colParameters);

                result = 1;
            }
            catch (Exception)
            {
                return result;
                throw;
            }

            return result;
        }

        #endregion

    }
}