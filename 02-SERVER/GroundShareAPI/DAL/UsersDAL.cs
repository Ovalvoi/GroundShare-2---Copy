using GroundShare.BL;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace GroundShare.DAL
{
    public class UsersDAL : DBServices
    {
        private SqlConnection connection;
        private SqlCommand command;
        private SqlDataReader reader;

        // -----------------------------------------------------------
        // Login: החזרת משתמש מלא לפי אימייל וסיסמה
        // משתמש ב-SqlDataReader (יעיל יותר מ-DataTable)
        // -----------------------------------------------------------
        public User Login(string email, string password)
        {
            User user = null;

            try
            {
                connection = Connect();

                Dictionary<string, object> paramDic = new Dictionary<string, object>();
                paramDic.Add("@Email", email);
                paramDic.Add("@Password", password);

                command = CreateCommandWithStoredProcedure("spLoginUser", connection, paramDic);

                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    user = new User();
                    user.UserId = Convert.ToInt32(reader["UserId"]);
                    user.FirstName = reader["FirstName"].ToString();
                    user.LastName = reader["LastName"].ToString();
                    user.Email = reader["Email"].ToString();
                    user.IsAdmin = Convert.ToBoolean(reader["IsAdmin"]);
                    // שים לב: אנחנו לא מחזירים סיסמה לקליינט מטעמי אבטחה
                }
            }
            finally
            {
                if (reader != null && !reader.IsClosed) reader.Close();
                if (connection != null && connection.State != ConnectionState.Closed) connection.Close();
            }

            return user;
        }

        // -----------------------------------------------------------
        // Register: רישום עם פרמטרים בודדים (כמו בפרויקט הדוגמה)
        // -----------------------------------------------------------
        public int Register(string firstName, string lastName, string email, string password, DateTime dateOfBirth, string phoneNumber, string address)
        {
            int newId = -1;

            try
            {
                connection = Connect();

                Dictionary<string, object> paramDic = new Dictionary<string, object>();
                paramDic.Add("@FirstName", firstName);
                paramDic.Add("@LastName", lastName);
                paramDic.Add("@Email", email);
                paramDic.Add("@Password", password);
                paramDic.Add("@DateOfBirth", dateOfBirth);
                paramDic.Add("@PhoneNumber", phoneNumber);
                paramDic.Add("@Address", address);

                command = CreateCommandWithStoredProcedure("spRegisterUser", connection, paramDic);

                // ה-SP אמור להחזיר את ה-ID החדש או -1 אם קיים
                object scalar = command.ExecuteScalar();

                if (scalar != null && scalar != DBNull.Value)
                {
                    if (scalar is decimal dec)
                        newId = (int)dec;
                    else
                        newId = Convert.ToInt32(scalar);
                }
            }
            finally
            {
                if (connection != null && connection.State != ConnectionState.Closed) connection.Close();
            }

            return newId;
        }
    }
}