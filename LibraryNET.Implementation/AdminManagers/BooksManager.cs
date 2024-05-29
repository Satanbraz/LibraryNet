using DTO;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace LibraryNET.Implementation.AdminManagers
{
    public class BooksManager
    {
        //Agregar conexion a BD
        DbConnectionProvider dbc = new DbConnectionProvider();

        #region Propierties

        protected string TableName = "Books";
        protected string Fields = "Id, BookTitle, BookAuthor, BookEditId, BookYear, BookGenderId, BookStateId, BookBC, BookCant";

        #endregion

        #region Public Methods
        public int Create(Books books)
        {
            // Definir la query
            string query = string.Format("INSERT INTO {0} ({1}) {2}",
                            TableName,
                            "BookTitle, BookAuthor, BookEditId, BookYear, BookGenderId, BookStateId, BookBC, BookCant, BookPrice",
                            "VALUES(@BookTitle, @BookAuthor, @BookEditId, @BookYear, @BookGenderId, @BookStateId, @BookBC, @BookCant, @BookPrice)");

            // Definir los paramatros de enviar

            List<SqlParameter> colParameters = new List<SqlParameter>();
            colParameters.Add(new SqlParameter("BookTitle", books.Title));
            colParameters.Add(new SqlParameter("BookAuthor", books.Author));
            colParameters.Add(new SqlParameter("BookYear", books.BookYear));
            colParameters.Add(new SqlParameter("BookGenderId", books.BookGenderId));
            colParameters.Add(new SqlParameter("BookStateId", books.BookStateId));
            colParameters.Add(new SqlParameter("BookEditId", books.BookEditId));
            colParameters.Add(new SqlParameter("BookBC", books.BookBC));
            colParameters.Add(new SqlParameter("BookCant", books.Stock));
            colParameters.Add(new SqlParameter("BookPrice", books.BookPrice));

            //Ejecutar el comando y obtener el resultado
            dbc = new DbConnectionProvider();

            int id = Convert.ToInt32(dbc.ExecuteReturningId(query, "Id", colParameters,TableName));

            return id;
        }

        public Books Read(int bookId)
        {

            string query = string.Format("SELECT {0} FROM {1} WHERE {2}",
                                        Fields,
                                        TableName,
                                        bookId);

            dbc = new DbConnectionProvider();
            DataTable data = dbc.GetDataTable(query);

            //Convertir y retornar la data
            Books booksList = new Books();

            if ((data != null) && (data.Rows.Count > 0))
            {
                return booksList = ConvertDtRowToDTO(data.Rows[0]);
            }
            else
            {
                return null;
            }
        }

        public int Update(Books books)
        {
            throw new NotImplementedException();
        }

        public void Delete(int bookId)
        {
            string query = string.Format(@"DELETE FROM {0}
                                        WHERE Id = {1}",
                                        TableName,
                                        bookId);
            // SqlCommand cmd = new SqlCommand(query, dbc.GetConnection());
            // cmd.ExecuteReader();

        }

        public IEnumerable<Books> List()
        {
            string query = string.Format(@"SELECT 
                                        A.Id, 
                                        BookTitle, 
                                        BookAuthor, 
                                        BookYear, 
                                        B.GenderName as BookGender, 
                                        C.StateName as BookState, 
                                        D.EditName as BookEdit, 
                                        BookBC, 
                                        BookCant, 
                                        BookPrice, 
                                        BookImage 
                                        FROM {0} as A 
                                        LEFT JOIN Genders as B on B.Id = A.BookGenderId 
                                        LEFT JOIN BorrowStates as C on C.Id = A.BookStateId 
                                        LEFT JOIN EditBook as D on D.Id = A.BookEditId ",
                                        TableName);

            // Crear conexion y ejecutar la query
            dbc = new DbConnectionProvider();
            DataTable data = dbc.GetDataTable(query);

            // Crear lista de registros
            List<Books> booksList = new List<Books>();

            // Rellenar lista con registros
            if ((data != null) && (data.Rows.Count > 0))
            {
                for (int i = 0; i < data.Rows.Count; i++)
                {
                    booksList.Add(ConvertDtRowToDTO(data.Rows[i]));
                }
            }

            return booksList;

        }

        public IList<Books> ListFiltered(int bookGenderId, string busqueda) {

            string queryWhere = string.Empty;

            if (bookGenderId > 0 && string.IsNullOrEmpty(busqueda))
            {
                queryWhere = " WHERE BookGenderId = " + bookGenderId;
            }

            if (bookGenderId <=0 && !string.IsNullOrEmpty(busqueda))
            {
                queryWhere = " WHERE BookTitle LIKE '%" + busqueda + "%'";
            }

            string query = string.Format(@"SELECT 
                                        A.Id, 
                                        BookTitle, 
                                        BookAuthor, 
                                        BookYear, 
                                        B.GenderName as BookGender, 
                                        C.StateName as BookState, 
                                        D.EditName as BookEdit, 
                                        BookBC, 
                                        BookCant, 
                                        BookPrice, 
                                        BookImage 
                                        FROM {0} as A 
                                        LEFT JOIN Genders as B on B.Id = A.BookGenderId 
                                        LEFT JOIN BorrowStates as C on C.Id = A.BookStateId 
                                        LEFT JOIN EditBook as D on D.Id = A.BookEditId 
                                        {1}",
                                        TableName,
                                        queryWhere);

            // Crear conexion y ejecutar la query
            dbc = new DbConnectionProvider();
            DataTable data = dbc.GetDataTable(query);

            // Crear lista de registros
            List<Books> booksList = new List<Books>();

            // Rellenar lista con registros
            if ((data != null) && (data.Rows.Count > 0))
            {
                for (int i = 0; i < data.Rows.Count; i++)
                {
                    booksList.Add(ConvertDtRowToDTO(data.Rows[i]));
                }
            }

            return booksList;
        }

        public void SaveBookImage(int bookId, byte[] imageData)
        {
           
            // Convertir la imagen a una cadena Base64
            string base64Image = Convert.ToBase64String(imageData);

            try
            {
                // Consulta SQL con parámetros
                string command = @"UPDATE " + TableName + @" 
                            SET BookImage = @Base64Image
                            WHERE Id = @bookId";


                //Definir los parámetros
                List<SqlParameter> colParameters = new List<SqlParameter>();
                colParameters.Add(new SqlParameter("bookId", bookId));
                colParameters.Add(new SqlParameter("Base64Image", base64Image));

                //Ejecutar el comando
                DbConnectionProvider tdBase = new DbConnectionProvider();
                tdBase.Execute(command, colParameters);
         
            }
            catch (Exception ex)
            {
                throw;
            }
            
        }


        #endregion

        #region Protected Methods

        protected Books ConvertDtRowToDTO(DataRow data)
        {
            Books book = new Books
            {
                Id = (int)data["Id"],
                Title = data["BookTitle"].ToString(),
                Author = data["BookAuthor"].ToString(),
                BookEdit = data["BookEdit"].ToString(),
                BookYear = Convert.IsDBNull(data["BookYear"]) ? 0 : (int)data["BookYear"],
                BookGender = data["BookGender"].ToString(),
                BookState = data["BookState"].ToString(),
                BookBC = data["BookBC"].ToString(),
                Stock = (int)data["BookCant"],
                BookPrice = Convert.IsDBNull(data["BookPrice"]) ? 0 : (int)data["BookPrice"],
                BookImage = Convert.FromBase64String(data["BookImage"].ToString()),
            };

            return book;
        }

        #endregion
    }
}