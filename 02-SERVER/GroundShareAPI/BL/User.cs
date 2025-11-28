using System;
using System.Collections.Generic;
using System.Data;
using GroundShare.DAL;

namespace GroundShare.BL
{
    public class User
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public bool IsAdmin { get; set; }

        public User() { }

        // Login
        public User Login(string email, string password)
        {
            UsersDAL dal = new UsersDAL();
            DataTable dt = dal.Login(email, password);

            if (dt.Rows.Count > 0)
            {
                User u = new User();
                u.UserId = Convert.ToInt32(dt.Rows[0]["UserId"]);
                u.FirstName = dt.Rows[0]["FirstName"].ToString();
                u.LastName = dt.Rows[0]["LastName"].ToString();
                u.Email = dt.Rows[0]["Email"].ToString();
                u.IsAdmin = Convert.ToBoolean(dt.Rows[0]["IsAdmin"]);
                return u;
            }

            return null;
        }

        // Register
        public int Insert()
        {
            UsersDAL dal = new UsersDAL();
            return dal.Register(this.FirstName, this.LastName, this.Email, this.Password, this.DateOfBirth, this.PhoneNumber, this.Address);
        }
    }
}