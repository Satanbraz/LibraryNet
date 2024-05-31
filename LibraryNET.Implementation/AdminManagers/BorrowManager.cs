using DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace LibraryNET.Implementation.AdminManagers
{
    public class BorrowManager
    {
        //Agregar conexion a BD
        DbConnectionProvider dbc = new DbConnectionProvider();

        #region Propierties

        protected string TableName = "Borrows";
        protected string Fields = "Id, UserId, IdTransaction, ProductsQ, UserPhone, DirUser, BorrowRegisterDate";

        #endregion

        #region PublicMethods

        public int InsertBorrowRequest(Borrows borrow, DataTable borrowDetail)
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
                                    "UserId, IdTransaction, ProductsQ, UserPhone, DirUser, BorrowRegisterDate",
                                    "VALUES(@UserId, @IdTransaction, @ProductsQ, @UserPhone, @DirUser, @BorrowRegisterDate)");

                    // Definir los paramatros de enviar
                    List<SqlParameter> colParameters = new List<SqlParameter>();
                    colParameters.Add(new SqlParameter("UserId", borrow.IdUser));
                    colParameters.Add(new SqlParameter("IdTransaction", borrow.ProductsQ));
                    colParameters.Add(new SqlParameter("ProductsQ", borrow.PhoneUser));
                    colParameters.Add(new SqlParameter("UserPhone", borrow.DirUser));
                    colParameters.Add(new SqlParameter("DirUser", borrow.IdTransaction));;
                    colParameters.Add(new SqlParameter("BorrowRegisterDate", DateTime.Now));


                    //Ejecutar el comando y obtener el resultado
                    dbc = new DbConnectionProvider();
                    int id = Convert.ToInt32(dbc.ExecuteReturningId(query, "Id", colParameters, TableName));

                    if (id > 0)
                    {
                        try
                        {
                            // Crear lista de BuyDetail
                            List<BorrowDetail> borrowDetailList = new List<BorrowDetail>();

                            // Rellenar lista con registros
                            if ((borrowDetail != null) && (borrowDetail.Rows.Count > 0))
                            {
                                try
                                {
                                    for (int i = 0; i < borrowDetail.Rows.Count; i++)
                                    {
                                        // Transformar las filas del DT en fila de BuyDetail
                                        BorrowDetail borrowDetailObj = ConvertDtRowToDTO(borrowDetail.Rows[i]);
                                        borrowDetailObj.BorrowId = id;
                                        borrowDetailList.Add(borrowDetailObj);

                                        // Valida la cantidad comprada de cada libro
                                        if (borrowDetailObj.BorrowBookQ > 1)
                                        {
                                            try
                                            {
                                                result = updateBookStock(borrowDetailObj.BorrowBookQ, borrowDetailObj.BorrowProductId);
                                            }
                                            catch (Exception)
                                            {

                                                throw;
                                            }
                                        }
                                        // Genera un registro en el Detalle de Compra
                                        result = InsertBorrowDetail(borrowDetailList[i]);
                                    }

                                    if (result == 1)
                                    {
                                        try
                                        {
                                            // Elimina el carrito generado al usuario
                                            result = delBorrowCart(borrow.IdUser);
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

        public IEnumerable<Borrows> GetBorrows(int idUser)
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
		                                FROM Buys as A 
		                                LEFT JOIN BuyDetail as B on A.Id = B.BuyId
		                                LEFT JOIN Books as C on C.Id = B.ProductId 
                                        WHERE A.IdUser = {0}",
                                        idUser);

            // Crear conexion y ejecutar la query
            dbc = new DbConnectionProvider();
            DataTable data = dbc.GetDataTable(query);

            // Crear lista de registros
            List<Borrows> buyDetailList = new List<Borrows>();

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
        protected BorrowDetail ConvertDtRowToDTO(DataRow data)
        {
            BorrowDetail buyDetail = new BorrowDetail
            {
                BorrowProductId = (int)data["IdProduct"],
                BorrowBookQ = (int)data["QProducto"],
                BorrowDate = DateTime.Parse(data["BuyDate"].ToString()),
                BorrowReturnDate = DateTime.Parse(data["BuyDate"].ToString())
            };

            return buyDetail;
        }

        // Convierte el DT en un objeto Clase Buy
        protected Borrows ConvertDtRowToDTOGet(DataRow data)
        {
            Borrows borrowDetail = new Borrows
            {
                Id = (int)data["Id"],
                IdTransaction = data["IdTransaction"].ToString(),
                BorrowRegisterDate = DateTime.Parse(data["BuyDate"].ToString()),
                borrowDetail = new BorrowDetail()
                {
                    Id = (int)data["BDetId"],
                    BorrowId = (int)data["BuyId"],
                    BorrowProductId = (int)data["ProductId"],
                    BorrowBookQ = (int)data["QProduct"],
                },
                books = new Books()
                {
                    Title = data["BookTitle"].ToString(),
                    Author = data["BookAuthor"].ToString(),
                    BookImage = Convert.FromBase64String(data["BookImage"].ToString())
                }
            };

            return borrowDetail;
        }

        // Metodo para agregar un registro en BuyDetail
        protected int InsertBorrowDetail(BorrowDetail borrowDetail)
        {
            int result = 0;

            try
            {
                // Definir la query
                string query = string.Format("INSERT INTO BorrowDetail ({0}) {1}",
                                "BorrowId, ProductId, QProduct, BorrowDate, BorrowReturnDate",
                                "VALUES(@BorrowId, @ProductId, @QProduct, @BorrowDate, @BorrowReturnDate)");

                // Definir los paramatros de enviar
                List<SqlParameter> colParameters = new List<SqlParameter>();
                colParameters.Add(new SqlParameter("BorrowId", borrowDetail.BorrowId));
                colParameters.Add(new SqlParameter("ProductId", borrowDetail.BorrowProductId));
                colParameters.Add(new SqlParameter("QProduct", borrowDetail.BorrowBookQ));
                colParameters.Add(new SqlParameter("BorrowDate", DateTime.Parse(borrowDetail.BorrowDate.ToString("yyyy-mm-dd"))));
                colParameters.Add(new SqlParameter("BorrowReturnDate", DateTime.Parse(borrowDetail.BorrowReturnDate.ToString("yyyy-mm-dd"))));

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

        // Metodo para eliminar el carrito de prestamos del usuario
        protected int delBorrowCart(int idUser)
        {
            int result = 0;
            try
            {
                //Definir el comando
                string command = string.Format("DELETE FROM BorrowCart WHERE IdUser = {0}",
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

        // Metodo para actualizar el stock del libro prestado
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