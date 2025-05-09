using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrativeProgramming_UI.Models
{
    internal class User
    {
        public string UserID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string UserHash { get; set; }
        public string UserRole { get; set; }
        public bool IsActive { get; set; }
        public bool IsNew { get; set; }
        public DateTime CreatedAt { get; set; }

        public User()
        {
            
        }
        public User(string userid, string username, string password, string hash, string 
            role, bool isActive, bool isNew, DateTime createdAt)
        {
            UserID = userid;
            Username = username;
            Password = password;
            UserHash = hash;
            UserRole = role;
            IsActive = isActive;
            IsNew = isNew;
            CreatedAt = createdAt;
        }

    }
}
