using DTO;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Security.Cryptography;

namespace LibraryNET.Implementation.AdminManagers
{
    public class CartManager
    {
        //Agregar conexion a BD
        DbConnectionProvider dbc = new DbConnectionProvider();

        #region Propierties

        protected string TableName = "Cart";
        protected string Fields = "Id, IdUser, IdBook";

        BooksManager booksManager = new BooksManager();

        #endregion

        #region Public Methods

        // Método para agregar un libro al carrito
        public int AddBookToCart(Cart cart)
        {
            int result = 0;

            //Comprobar que no exista el libro en el carrito.
            //Definir la query   
            string query = string.Format("SELECT {0} FROM {1} WHERE IdBook = {2} AND IdUser = {3}",
                                         Fields,
                                         TableName,
                                         "@IdBook",
                                         "@IdUser");

            //Definir los parámetros
            List<SqlParameter> colParameters = new List<SqlParameter>();
            colParameters.Add(new SqlParameter("IdBook", cart.IdProduct));
            colParameters.Add(new SqlParameter("IdUser", cart.IdUser));

            //Ejecutar la query y obtener el resultado
            DbConnectionProvider tdBase = new DbConnectionProvider();
            DataTable data = tdBase.GetDataTable(query, colParameters);

            //Convertir y retornar la data
            if ((data == null) || (data.Rows.Count == 0))
            {
                try
                {
                    //Definir el comando
                    string command = string.Format("UPDATE Books SET {0} WHERE Id = {1}",
                                                   "BookCant = BookCant - 1",
                                                   "@Id");

                    //Definir los parámetros
                    colParameters = new List<SqlParameter>();
                    colParameters.Add(new SqlParameter("Id", cart.IdProduct));

                    //Ejecutar el comando
                    tdBase = new DbConnectionProvider();
                    tdBase.Execute(command, colParameters);

                    try {
                        //Comprobar que no exista el libro en el carrito.
                        //Definir la query   
                        query = string.Format("INSERT INTO {0} ({1}) {2}",
                                                     TableName,
                                                     "IdBook, IdUser",
                                                     "VALUES(@IdBook, @IdUser)");

                        //Definir los parámetros
                        colParameters = new List<SqlParameter>();
                        colParameters.Add(new SqlParameter("IdBook", cart.IdProduct));
                        colParameters.Add(new SqlParameter("IdUser", cart.IdUser));

                        //Ejecutar el comando
                        tdBase = new DbConnectionProvider();
                        tdBase.Execute(query, colParameters);

                        result = 1;
                    }
                    catch {
                    
                    }
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            return result;
        }

        // Método para eliminar un libro del carrito
        public int RemoveBookFromCart(int idCart, int bookId)
        {
            int result = 0;
            try
            {
                //Definir el comando
                string query = string.Format("DELETE FROM {0} WHERE Id = {1}",
                                               TableName,
                                               "@Id");

                //Definir los parámetros
                List<SqlParameter> colParameters = new List<SqlParameter>();
                colParameters.Add(new SqlParameter("Id", idCart));

                //Ejecutar el comando
                DbConnectionProvider tdBase = new DbConnectionProvider();
                tdBase.Execute(query, colParameters);

                try
                {
                    //Definir el comando
                    string command = string.Format("UPDATE Books SET {0} WHERE Id = {1}",
                                                    "BookCant = BookCant + 1",
                                                    "@Id");


                    //Definir los parámetros
                    colParameters = new List<SqlParameter>();
                    colParameters.Add(new SqlParameter("Id", bookId));

                    //Ejecutar el comando
                    tdBase = new DbConnectionProvider();
                    tdBase.Execute(command, colParameters);

                    result = 1;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            

            return result;
        }

        // Método para obtener el carrito
        public IEnumerable<Cart> GetCartDetail(int userId)
        {
            string query = string.Format(@"SELECT 
                                        A.Id, 
                                        B.Id AS IdBook,
                                        B.BookTitle, 
                                        B.BookAuthor, 
                                        B.BookYear, 
                                        B.BookBC,
                                        B.BookCant,
                                        B.BookPrice, 
                                        B.BookImage 
                                        FROM {0} as A 
                                        LEFT JOIN Books as B on B.Id = A.IdBook
                                        WHERE A.IdUser = {1}",
                                        TableName,
                                        userId);

            // Crear conexion y ejecutar la query
            dbc = new DbConnectionProvider();
            DataTable data = dbc.GetDataTable(query);

            // Crear lista de registros
            List<Cart> cartList = new List<Cart>();

            // Rellenar lista con registros
            if ((data != null) && (data.Rows.Count > 0))
            {
                for (int i = 0; i < data.Rows.Count; i++)
                {
                    cartList.Add(ConvertDtRowToDTO(data.Rows[i]));
                }
            }

            return cartList;
        }

        // Metodo para verificar si hay un carrito del usuario
        public int VarifyCart(int userId)
        {
            int result = 0;

            string query = string.Format("SELECT {0} FROM {1} WHERE IdUser = {2}",
                                       Fields,
                                       TableName,
                                       userId);

            dbc = new DbConnectionProvider();
            DataTable data = dbc.GetDataTable(query);

            if ((data != null) && (data.Rows.Count > 0))
            {
                return result = 1;
            }
            else
            {
                return result;
            }
        }

        #endregion

        #region Protected Methods

        protected Cart ConvertDtRowToDTO(DataRow data)
        {
            Cart book = new Cart
            {
                Id = (int)data["Id"],
                Books = new Books()
                {
                    Id = (int)data["IdBook"],
                    Title = data["BookTitle"].ToString(),
                    Author = data["BookAuthor"].ToString(),
                    BookYear = Convert.IsDBNull(data["BookYear"]) ? 0 : (int)data["BookYear"],
                    BookBC = data["BookBC"].ToString(),
                    Stock = (int)data["BookCant"],
                    BookPrice = Convert.IsDBNull(data["BookPrice"]) ? 0 : (int)data["BookPrice"],
                    BookImage = Convert.FromBase64String(data["BookImage"].ToString()),
                }
            };

            return book;
        }

        #endregion
    }
}