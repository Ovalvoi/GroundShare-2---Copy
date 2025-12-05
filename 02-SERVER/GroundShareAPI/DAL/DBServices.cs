using Microsoft.Data.SqlClient;

namespace GroundShare.DAL
{
    // מחלקה זו משמשת כשירות בסיס לכל ה-DALים בפרויקט
    // היא אחראית על יצירת החיבור למסד הנתונים והרצת הפרוצדורות
    public class DBServices
    {
        // ---------------------------------------------------------------------------------
        // יצירת אובייקט חיבור (SqlConnection) למסד הנתונים
        // הפונקציה קוראת את מחרוזת החיבור מתוך קובץ ההגדרות appsettings.json
        // ---------------------------------------------------------------------------------
        protected SqlConnection Connect()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            // שליפת המחרוזת בשם DefaultConnection
            string cStr = configuration.GetConnectionString("DefaultConnection");

            SqlConnection con = new SqlConnection(cStr);
            con.Open(); // פתיחת החיבור
            return con;
        }

        // ---------------------------------------------------------------------------------
        // יצירת אובייקט פקודה (SqlCommand) המקושר ל-Stored Procedure
        // מקבל: שם הפרוצדורה, החיבור הפתוח, ומילון של פרמטרים (אם יש)
        // ---------------------------------------------------------------------------------
        protected SqlCommand CreateCommandWithStoredProcedure(string spName, SqlConnection con, Dictionary<string, object> paramDic)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = spName;
            cmd.CommandTimeout = 10; // זמן המתנה מקסימלי לתשובה (בשניות)
            cmd.CommandType = System.Data.CommandType.StoredProcedure; // הגדרה שמדובר ב-SP

            // אם נשלחו פרמטרים, נוסיף אותם לפקודה
            if (paramDic != null)
            {
                //key value pair means that each parameter has a name (key) and a value (value) for example:
                //param is a KeyValuePair.
                //param.Key = "@City"
                //param.Value = "Tel Aviv"
                foreach (KeyValuePair<string, object> param in paramDic)
                {
                    // שימוש ב-DBNull.Value אם הערך הוא null, כדי למנוע שגיאות SQL
                    cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                }
            }
            return cmd;
        }
    }
}