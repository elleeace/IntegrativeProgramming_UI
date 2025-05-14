using IntegrativeProgramming_UI.Helpers;
using IntegrativeProgramming_UI.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace IntegrativeProgramming_UI.Services
{
    internal class UserService
    {
        private readonly NorthvilleLibraryDataContext db = new NorthvilleLibraryDataContext(Properties.Settings.Default.NorthvilleLibraryV2ConnectionString);

        public UserService(NorthvilleLibraryDataContext db)
        {
            this.db = db;
        }

        public void EditUser(StackPanel formPanel, NorthvilleLibraryDataContext db, User user, Action onSaved)
        {
            FormBuilder.BuildEditUserForm(formPanel, db, user.UserID, onSaved);
        }

        public ObservableCollection<User> LoadUserTable()
        {
            return new ObservableCollection<User>(
                (from ut in db.users
                 select new User
                 {
                     UserID = ut.user_id,
                     Username = ut.username,
                     Password = ut.user_password,
                     UserHash = ut.user_hash,
                     UserRole = ut.user_role,
                     IsActive = ut.is_active,
                     IsNew = ut.is_new,
                     CreatedAt = ut.created_at
                 }).ToList()
            );
        }


        public void AddUser(StackPanel sp, Action onSuccess)
        {
            FormBuilder.BuildAddUserForm(sp,db,onSuccess);
        }

        public void DeleteUser(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId)) return;

            var user = db.users.FirstOrDefault(u => u.user_id == userId);
            if (user == null)
            {
                MessageBoxBuilder.ShowError("User not found.");
                return;
            }

            db.users.DeleteOnSubmit(user);
            CRUDHelper.SafeSubmit(db); // Use your centralized helper
        }

    }
}
