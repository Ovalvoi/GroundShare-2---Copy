using GroundShare.BL;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Collections.Generic;
using System;

namespace GroundShare.DAL
{
    public class UsersDAL : DBServices
    {
        // Removed class-level fields (connection, command, reader) to ensure thread safety
        // הסרתי את המשתנים ברמת המחלקה כדי למנוע התנגשויות

        // -----------------------------------------------------------
        // Login: החזרת משתמש מלא לפי אימייל וסיסמה
        // משתמש ב-SqlDataReader (יעיל יותר מ-DataTable)
        // -----------------------------------------------------------
        public User Login(string email, string password)
        {
            User user = null;

            using (SqlConnection connection = Connect())
            {
                Dictionary<string, object> paramDic = new Dictionary<string, object>
                {
                    { "@Email", email },
                    { "@Password", password }
                };

                using (SqlCommand command = CreateCommandWithStoredProcedure("spLoginUser", connection, paramDic))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
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
                }
            }

            return user;
        }

        // -----------------------------------------------------------
        // Register: רישום עם פרמטרים בודדים (כמו בפרויקט הדוגמה)
        // -----------------------------------------------------------
        public int Register(string firstName, string lastName, string email, string password, DateTime dateOfBirth, string phoneNumber, string address)
        {
            int newId = -1;

            using (SqlConnection connection = Connect())
            {
                Dictionary<string, object> paramDic = new Dictionary<string, object>
                {
                    { "@FirstName", firstName },
                    { "@LastName", lastName },
                    { "@Email", email },
                    { "@Password", password },
                    { "@DateOfBirth", dateOfBirth },
                    { "@PhoneNumber", phoneNumber },
                    { "@Address", address }
                };

                using (SqlCommand command = CreateCommandWithStoredProcedure("spRegisterUser", connection, paramDic))
                {
                    // ExecuteReader
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // שימוש ב-Convert.ToInt32 מבצע המרה אוטומטית, גם אם ה-SQL מחזיר Decimal (מה שקורה עם SCOPE_IDENTITY)
                            // for example, if the database returns a decimal (LIKE:5.0) type for the new ID we can still convert it to int
                            if (reader[0] != DBNull.Value)
                            {
                                newId = Convert.ToInt32(reader[0]);
                            }
                        }
                    }
                }
            }

            return newId;
        }
    }
}