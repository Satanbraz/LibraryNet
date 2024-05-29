using DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace LibraryNET.Implementation
{
    public class UserManager
    {
        //Agregar conexion a BD
        DbConnectionProvider dbc = new DbConnectionProvider();

        #region Propierties

        protected string TableName = "Users";
        protected string Fields = "Id,UserName,UserLName,UserEmail,UserPass,UserPhone,UserDir,UserDate";

        #endregion

        #region UserMethods
        public int Create(Users user)
        {
            // Definir la query
            string query = string.Format("INSERT INTO {0} ({1}) {2}",
                            TableName,
                            "UserName, UserLName, UserEmail, UserPass, UserPhone, UserDir, RolId, UserDate",
                            "VALUES(@UserName, @UserLName, @UserEmail, @UserPass, @UserPhone ,@UserDir, @RolId, @UserDate)");

            // Definir los paramatros de enviar

            List<SqlParameter> colParameters = new List<SqlParameter>();
            colParameters.Add(new SqlParameter("UserName", user.UserName));
            colParameters.Add(new SqlParameter("UserLName", user.UserLastName));
            colParameters.Add(new SqlParameter("UserEmail", user.UserEmail));
            colParameters.Add(new SqlParameter("UserPass", user.UserPassword));
            colParameters.Add(new SqlParameter("UserPhone", user.UserPhone));
            colParameters.Add(new SqlParameter("UserDir", user.UserDir));
            colParameters.Add(new SqlParameter("RolId", user.UserRolId));
            colParameters.Add(new SqlParameter("UserDate", DateTime.Now.Date));

            //Ejecutar el comando y obtener el resultado
            dbc = new DbConnectionProvider();
            int id = Convert.ToInt32(dbc.ExecuteReturningId(query, "Id", colParameters, TableName));

            return id;
        }

        public Users Read(int UserId)
        {

            string query = string.Format(@"SELECT 
                                        Id, 
                                        UserName, 
                                        UserLName, 
                                        UserEmail,
                                        RolId, 
                                        UserPass, 
                                        UserPhone, 
                                        UserDir  
                                        FROM {0}
                                        Where Id = {1}",
                                        TableName,
                                        UserId);

            dbc = new DbConnectionProvider();
            DataTable data = dbc.GetDataTable(query);

            //Convertir y retornar la data
            Users usersList = new Users();

            if ((data != null) && (data.Rows.Count > 0))
            {
                return usersList = ConvertDtRowReadToDTO(data.Rows[0]);
            }
            else
            {
                return null;
            }
        }

        public int Update(Users users)
        {
            int result = 0;

            //Comprobar que no existan Editoriales con el mismo nombre.
            //Definir la query   
            string query = string.Format("SELECT {0} FROM {1} WHERE UserName = {2} AND UserLName = {3} AND UserEmail = {4} AND UserPhone = {5} AND UserDir = {6} AND RolId = {7}",
                                         Fields,
                                         TableName,
                                         "@UserName", "@UserLName", "@UserEmail", "@UserPhone", "@UserDir", "@RolId");

            //Definir los parámetros
            List<SqlParameter> colParameters = new List<SqlParameter>();
            colParameters.Add(new SqlParameter("UserName", users.UserName));
            colParameters.Add(new SqlParameter("UserLName", users.UserLastName));
            colParameters.Add(new SqlParameter("UserEmail", users.UserEmail));
            colParameters.Add(new SqlParameter("UserPhone", users.UserPhone));
            colParameters.Add(new SqlParameter("UserDir", users.UserDir));
            colParameters.Add(new SqlParameter("RolId", users.UserRolId));

            //Ejecutar la query y obtener el resultado
            DbConnectionProvider tdBase = new DbConnectionProvider();
            DataTable data = tdBase.GetDataTable(query, colParameters);

            //Convertir y retornar la data
            if ((data == null) || (data.Rows.Count == 0))
            {
                try
                {
                    //Definir el comando
                    string command = string.Format("UPDATE {0} \nSET \n{1} \nWHERE Id = {2}",
                                                   TableName,
                                                   "UserName = @UserName, \n" +
                                                   "UserLName = @UserLName, \n" +
                                                   "UserEmail = @UserEmail, \n" +
                                                   "UserPhone = @UserPhone, \n" +
                                                   "UserDir = @UserDir, \n" +
                                                   "RolId = @RolId",
                                                   "@Id");


                    //Definir los parámetros
                    colParameters = new List<SqlParameter>();
                    colParameters.Add(new SqlParameter("UserName", users.UserName));
                    colParameters.Add(new SqlParameter("UserLName", users.UserLastName));
                    colParameters.Add(new SqlParameter("UserEmail", users.UserEmail));
                    colParameters.Add(new SqlParameter("UserPhone", users.UserPhone));
                    colParameters.Add(new SqlParameter("UserDir", users.UserDir));
                    colParameters.Add(new SqlParameter("RolId", users.UserRolId));
                    colParameters.Add(new SqlParameter("Id", users.Id));

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
            return result;
        }

        public int Delete(int UserId)
        {
            int result = 0;
            try
            {
                //Definir el comando
                string command = string.Format("DELETE FROM {0} WHERE ID = {1}",
                                               TableName,
                                               "@Id");

                //Definir los parámetros
                List<SqlParameter> colParameters = new List<SqlParameter>();
                colParameters.Add(new SqlParameter("Id", UserId));

                //Ejecutar el comando
                DbConnectionProvider tdBase = new DbConnectionProvider();
                tdBase.Execute(command, colParameters);
                result = 1;
            }
            catch (Exception ex)
            {
                throw;
            }
            return result;

        }

        public IEnumerable<Users> List()
        {
            string query = string.Format(@"SELECT 
                                        A.Id, 
                                        UserName, 
                                        UserLName, 
                                        UserEmail,
                                        B.RolName as RolName, 
                                        UserPass, 
                                        UserPhone, 
                                        UserDir, 
                                        UserDate  
                                        FROM {0} as A 
                                        LEFT JOIN Rol AS B ON B.Id = A.RolId",
                                        TableName);

            // Crear conexion y ejecutar la query
            dbc = new DbConnectionProvider();
            DataTable data = dbc.GetDataTable(query);

            // Crear lista de registros
            List<Users> usersList = new List<Users>();

            // Rellenar lista con registros
            if ((data != null) && (data.Rows.Count > 0))
            {
                for (int i = 0; i < data.Rows.Count; i++)
                {
                    usersList.Add(ConvertDtRowToDTO(data.Rows[i]));
                }
            }

            return usersList;

        }

        public IEnumerable<Rol> RolList()
        {
            string query = string.Format(@"SELECT 
                                        Id, 
                                        RolName   
                                        FROM Rol");

            // Crear conexion y ejecutar la query
            dbc = new DbConnectionProvider();
            DataTable data = dbc.GetDataTable(query);

            // Crear lista de registros
            List<Rol> userRolList = new List<Rol>();

            // Rellenar lista con registros
            if ((data != null) && (data.Rows.Count > 0))
            {
                for (int i = 0; i < data.Rows.Count; i++)
                {
                    userRolList.Add(ConvertDtRowURToDTO(data.Rows[i]));
                }
            }

            return userRolList;
        }

        #endregion

        #region Protected Methods

        protected Users ConvertDtRowReadToDTO(DataRow data)
        {
            Users user = new Users
            {
                Id = (int)data["Id"],
                UserName = data["UserName"].ToString(),
                UserLastName = data["UserLName"].ToString(),
                UserEmail = data["UserEmail"].ToString(),
                UserRolId = (int)data["RolId"],
                UserPassword = data["UserPass"].ToString(),
                UserPhone = data["UserPhone"].ToString(),
                UserDir = data["UserDir"].ToString(),
            };

            return user;
        }
        protected Users ConvertDtRowToDTO(DataRow data)
        {
            Users user = new Users
            {
                Id = (int)data["Id"],
                UserRolName = data["RolName"].ToString(),
                UserName = data["UserName"].ToString(),
                UserLastName = data["UserLName"].ToString(),
                UserEmail = data["UserEmail"].ToString(),
                UserPassword = data["UserPass"].ToString(),
                UserPhone = data["UserPhone"].ToString(),
                UserDir = data["UserDir"].ToString(),
                UserDate = DateTime.Parse(data["UserDate"].ToString())
            };

            return user;
        }
        protected Rol ConvertDtRowURToDTO(DataRow data)
        {
            Rol user = new Rol
            {
                Id = (int)data["Id"],
                RolName = data["RolName"].ToString(),
            };

            return user;
        }
        #endregion

    }
}
