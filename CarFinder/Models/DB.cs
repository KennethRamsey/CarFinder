using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Configuration;


namespace CarFinder.Models
{

    public static class DB
    {
        /// <summary>
        /// get an already open connection to the database.
        /// </summary>
        /// <returns></returns>
        public static SqlConnection GetOpenConnection() {

            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            conn.Open();

            return conn;
        }

        /// <summary>
        /// generate a sqlCommand for a stored procedure.
        /// </summary>
        /// <param name="procedure">name of procedure as string</param>
        /// <param name="conn">connection to db</param>
        /// <returns></returns>
        public static SqlCommand StoredProc(string procedure, SqlConnection conn)
        {
            SqlCommand cmd = new SqlCommand(procedure, conn);
            cmd.CommandType = CommandType.StoredProcedure;

            return cmd;
        }


        /// <summary>
        /// Return list of strings for a sqldatareader.
        /// </summary>
        /// <param name="rdr"></param>
        /// <returns></returns>
        public static List<string> RdrToList(this SqlDataReader rdr)
        {
            List<string> values = new List<string>();

            while (rdr.Read())
                values.Add(rdr[0].ToString());

            return values;
        }
    }
}