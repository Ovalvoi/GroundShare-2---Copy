using System;
using System.Collections.Generic;
using System.Data;

namespace GroundShare.DAL
{
    public class UsersDAL
    {
        private DBServices _db;

        public UsersDAL()
        {
            _db = new DBServices();
        }

        public DataTable Login(string email, string password)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@Email", email },
                { "@Password", password }
            };

            return _db.ExecuteReader("spLoginUser", parameters);
        }

        public int Register(string firstName, string lastName, string email, string password, DateTime dateOfBirth, string phoneNumber, string address)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@FirstName", firstName },
                { "@LastName", lastName },
                { "@Email", email },
                { "@Password", password },
                { "@DateOfBirth", dateOfBirth },
                { "@PhoneNumber", phoneNumber },
                { "@Address", address }
            };

            return _db.ExecuteNonQuery("spRegisterUser", parameters);
        }
    }
}