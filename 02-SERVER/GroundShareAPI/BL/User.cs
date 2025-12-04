using System;
using System.Collections.Generic;
using GroundShare.DAL;

namespace GroundShare.BL
{
    public class User
    {
        public int UserId { get; set; }
        // סימן השאלה (?) אומר שהשדה יכול להיות ריק (Null)
        // זה קריטי כדי שלא נקבל שגיאה 400 בלוגין כששולחים רק אימייל וסיסמה
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public bool IsAdmin { get; set; }

        public User() { }

        // -----------------------------------------------------------
        // פונקציית התחברות (instance method)
        // מקבלת אימייל וסיסמה ומחזירה משתמש מלא או null
        // -----------------------------------------------------------
        public User Login(string email, string password)
        {
            UsersDAL dal = new UsersDAL();
            // כאן אנחנו קוראים ל-DAL שמחזיר אובייקט User מוכן
            return dal.Login(email, password);
        }

        // -----------------------------------------------------------
        // פונקציית הרשמה (instance method)
        // לוקחת את הנתונים מהמופע הנוכחי (this) ושולחת ל-DAL
        // -----------------------------------------------------------
        public int Insert()
        {
            UsersDAL dal = new UsersDAL();
            // שליחת השדות הבודדים ל-DAL כפי שהיה בפרויקט הדוגמה
            return dal.Register(
                this.FirstName,
                this.LastName,
                this.Email,
                this.Password,
                this.DateOfBirth,
                this.PhoneNumber,
                this.Address
            );
        }
    }
}