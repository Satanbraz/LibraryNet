using DTO;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;

namespace LibraryNET.Implementation
{
    public class GenderManager
    {
        //Agregar conexion a BD
        DbConnectionProvider dbc = new DbConnectionProvider();

        #region Propierties

        protected string TableName = "Genders";
        protected string Fields = "Id, GenderName";

        #endregion

        #region Public Methods
        public int Create(Gender gender)
        {
            // Definir la query
            string query = string.Format("INSERT INTO {0} ({1}) {2}",
                            TableName,
                            "GenderName",
                            "VALUES(@GenderName)");

            // Definir los paramatros de enviar

            List<SqlParameter> colParameters = new List<SqlParameter>();
            colParameters.Add(new SqlParameter("GenderName", gender.GenderName));

            //Ejecutar el comando y obtener el resultado
            dbc = new DbConnectionProvider();
            int id = Convert.ToInt32(dbc.ExecuteReturningId(query, "Id", colParameters, TableName));

            return id;
        }

        public Gender Read(int genderId)
        {

            string query = string.Format("SELECT {0} FROM {1} WHERE ID = {2}",
                                        Fields,
                                        TableName,
                                        genderId);

            dbc = new DbConnectionProvider();
            DataTable data = dbc.GetDataTable(query);

            //Convertir y retornar la data
            Gender gendersList = new Gender();

            if ((data != null) && (data.Rows.Count > 0))
            {
                return gendersList = ConvertDtRowToDTO(data.Rows[0]);
            }
            else
            {
                return null;
            }
        }

        public int Update(Gender gender)
        {
            int result = 0;

            //Comprobar que no existan Editoriales con el mismo nombre.
            //Definir la query   
            string query = string.Format("SELECT {0} FROM {1} WHERE GenderName = {2}",
                                         Fields,
                                         TableName,
                                         "@GenderName");

            //Definir los parámetros
            List<SqlParameter> colParameters = new List<SqlParameter>();
            colParameters.Add(new SqlParameter("GenderName", gender.GenderName));

            //Ejecutar la query y obtener el resultado
            DbConnectionProvider tdBase = new DbConnectionProvider();
            DataTable data = tdBase.GetDataTable(query, colParameters);

            //Convertir y retornar la data
            if ((data == null) || (data.Rows.Count == 0))
            {
                try
                {
                    //Definir el comando
                    string command = string.Format("UPDATE {0} SET {1} WHERE Id = {2}",
                                                   TableName,
                                                   "GenderName = @GenderName",
                                                   "@Id");


                    //Definir los parámetros
                    colParameters = new List<SqlParameter>();
                    colParameters.Add(new SqlParameter("GenderName", gender.GenderName));
                    colParameters.Add(new SqlParameter("Id", gender.Id));

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

        public int Delete(int genderId)
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
                colParameters.Add(new SqlParameter("Id", genderId));

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

        public IEnumerable<Gender> List()
        {
            string query = string.Format(@"SELECT 
                                        Id, 
                                        GenderName   
                                        FROM {0}",
                                        TableName);

            // Crear conexion y ejecutar la query
            dbc = new DbConnectionProvider();
            DataTable data = dbc.GetDataTable(query);

            // Crear lista de registros
            List<Gender> gendersList = new List<Gender>();

            // Rellenar lista con registros
            if ((data != null) && (data.Rows.Count > 0))
            {
                for (int i = 0; i < data.Rows.Count; i++)
                {
                    gendersList.Add(ConvertDtRowToDTO(data.Rows[i]));
                }
            }

            return gendersList;
        }

        #endregion

        #region Protected Methods

        protected Gender ConvertDtRowToDTO(DataRow data)
        {
            Gender gender = new Gender
            {
                Id = (int)data["Id"],
                GenderName = data["GenderName"].ToString()
            };

            return gender;
        }

        #endregion
    }
}