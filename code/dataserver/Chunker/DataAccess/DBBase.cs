using System;
using System.Data;
using System.Web.Configuration;

using MySql.Data.MySqlClient;

namespace Chunker.DataAccess
{
    public class DBBase
    {
        private string _connStr = WebConfigurationManager.ConnectionStrings["MySqlConn"].ConnectionString;

        protected DataSet Query(string sql)
        {
            using (MySqlConnection conn = new MySqlConnection(_connStr))
            {
                DataSet ds = new DataSet();
                try
                {
                    conn.Open();
                    MySqlDataAdapter command = new MySqlDataAdapter(sql, conn);
                    command.Fill(ds);
                    return ds;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    conn.Close();
                } 
            }
        }

        protected int ExecuteSql(string sql)
        {
            using (MySqlConnection connection = new MySqlConnection(_connStr))
            {
                using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                {
                    try
                    {
                        connection.Open();
                        int rows = cmd.ExecuteNonQuery();
                        return rows;
                    }
                    catch (System.Data.SqlClient.SqlException e)
                    {
                        connection.Close();
                        throw e;
                    }
                    finally
                    {
                        cmd.Dispose();
                        connection.Close();
                    }
                }
            }
        }
    }
}