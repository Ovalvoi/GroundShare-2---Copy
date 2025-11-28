using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration; // Needed for IConfiguration
using System.IO; // Needed for Directory.GetCurrentDirectory

namespace GroundShare.DAL
{
    public class DBServices
    {
        //--------------------------------------------------------------------------------------------------
        // 1. Create Connection (Teacher's Method)
        //--------------------------------------------------------------------------------------------------
        protected SqlConnection Connect()
        {
            // read the connection string from the configuration file
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()) // Use SetBasePath to ensure it finds the file
                .AddJsonFile("appsettings.json")
                .Build();

            string cStr = configuration.GetConnectionString("DefaultConnection");
            SqlConnection con = new SqlConnection(cStr);
            con.Open();
            return con;
        }

        //--------------------------------------------------------------------------------------------------
        // 2. Create Command (Teacher's Method)
        //--------------------------------------------------------------------------------------------------
        protected SqlCommand CreateCommandWithStoredProcedure(string spName, SqlConnection con, Dictionary<string, object> paramDic)
        {
            SqlCommand cmd = new SqlCommand(); // create the command object
            cmd.Connection = con;              // assign the connection to the command object
            cmd.CommandText = spName;          // can be Select, Insert, Update, Delete 
            cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds
            cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

            if (paramDic != null)
            {
                foreach (KeyValuePair<string, object> param in paramDic)
                {
                    // Use AddWithValue as per teacher's example, handling DBNull if value is null
                    cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                }
            }
            return cmd;
        }

        //--------------------------------------------------------------------------------------------------
        // 3. Execute Non Query (INSERT, UPDATE, DELETE)
        //--------------------------------------------------------------------------------------------------
        // Refactored to use the helper methods above
        public int ExecuteNonQuery(string spName, Dictionary<string, object> cmdParams)
        {
            SqlConnection con = null;
            try
            {
                con = Connect(); // Use the new Connect method
                SqlCommand cmd = CreateCommandWithStoredProcedure(spName, con, cmdParams); // Use the new helper

                // We use ExecuteScalar because our Insert SPs return the new ID (SELECT SCOPE_IDENTITY())
                object result = cmd.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    return Convert.ToInt32(result);
                }

                return 1; // Fallback
            }
            catch (Exception ex)
            {
                // In a real app, log the error
                throw ex;
            }
            finally
            {
                if (con != null) con.Close();
            }
        }

        //--------------------------------------------------------------------------------------------------
        // 4. Execute Reader (SELECT)
        //--------------------------------------------------------------------------------------------------
        // Refactored to use the helper methods above
        public DataTable ExecuteReader(string spName, Dictionary<string, object> cmdParams = null)
        {
            SqlConnection con = null;
            DataTable dt = new DataTable();

            try
            {
                con = Connect(); // Use the new Connect method
                SqlCommand cmd = CreateCommandWithStoredProcedure(spName, con, cmdParams); // Use the new helper

                SqlDataAdapter adptr = new SqlDataAdapter(cmd);
                adptr.Fill(dt);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (con != null) con.Close();
            }
            return dt;
        }
    }
}