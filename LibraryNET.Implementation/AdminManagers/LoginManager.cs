using DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace LibraryNET.Implementation
{
    public class LoginManager
    {
        DbConnectionProvider dbc = new DbConnectionProvider();

        #region Propierties

        protected string TableName = "Users";

        #endregion

        #region Public Methods
        public Users Read(string User, string Pass)
        {
            string query = string.Format(@"SELECT A.Id, 
                                        UserName, 
                                        UserLName, 
                                        UserEmail,
                                        RolId,
                                        B.RolName as RolName, 
                                        UserPass, 
                                        UserPhone, 
                                        UserDir, 
                                        UserDate 
                                        FROM {0} AS A 
                                        LEFT JOIN Rol AS B on B.Id = A.RolId 
                                        WHERE (UserPhone = '{1}' OR UserEmail = '{1}') AND UserPass = '{2}';",
                                        TableName,
                                        User,
                                        Pass);

            dbc = new DbConnectionProvider();
            DataTable data = dbc.GetDataTable(query);

            //Convertir y retornar la data
            Users usersList = new Users();

            if ((data != null) && (data.Rows.Count > 0))
            {
                return usersList = ConvertDtRowToDTO(data.Rows[0]);
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region Protected Methods

        protected Users ConvertDtRowToDTO(DataRow data)
        {
            Users user = new Users
            {
                Id = (int)data["Id"],
                UserRolId = (int)data["RolId"],
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

        #endregion


    }
}