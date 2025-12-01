using GroundShare.BL;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace GroundShare.DAL
{
    public class UsersDAL : DBServices
    {
        private SqlDataReader reader;
        private SqlConnection connection;
        private SqlCommand command;

        // רישום משתמש חדש - קריאה ל-SP: spRegisterUser
        // ה-SP מחזיר:
        // -1 אם האימייל כבר קיים
        // UserId (מספר הזהות החדש) אם נרשם בהצלחה
        public int RegisterUser(User user)
        {
            int result = -1;

            try
            {
                connection = Connect();

                Dictionary<string, object> paramDic = new Dictionary<string, object>();
                paramDic.Add("@FirstName", user.FirstName);
                paramDic.Add("@LastName", user.LastName);
                paramDic.Add("@Email", user.Email);
                paramDic.Add("@Password", user.Password);
                paramDic.Add("@DateOfBirth", user.DateOfBirth);
                paramDic.Add("@PhoneNumber", user.PhoneNumber);
                paramDic.Add("@Address", user.Address);

                command = CreateCommandWithStoredProcedure("spRegisterUser", connection, paramDic);

                // כי ה-SP מחזיר SELECT (-1 או SCOPE_IDENTITY())
                object scalar = command.ExecuteScalar();

                if (scalar != null && scalar != DBNull.Value)
                {
                    // SCOPE_IDENTITY מחזיר decimal בד"כ, לכן נזהר בהמרה
                    if (scalar is decimal dec)
                        result = (int)dec;
                    else
                        result = Convert.ToInt32(scalar);
                }
            }
            catch (SqlException ex)
            {
                // אם תחליטי להשתמש ב-RAISERROR ב-SP, אפשר לתפוס פה קוד שגיאה ספציפי
                throw;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (connection != null && connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
            }

            return result;
            // result == -1 → מייל כבר קיים
            // result > 0 → UserId של המשתמש החדש
        }
    

        // התחברות משתמש - קריאה ל-SP: spLoginUser
        // מחזיר אובייקט Users (ללא סיסמה) אם נמצאה התאמה, אחרת null
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

                    // לא מחזירים סיסמה – טוב מבחינת אבטחה
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                {
                    reader.Close();
                }

                if (connection != null && connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
            }

            return user; // null אם אין משתמש מתאים
        }
    }
}
