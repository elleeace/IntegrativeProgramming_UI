using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IntegrativeProgramming_UI
{
    public partial class MainWindow : Window
    {
        NorthvilleLibraryDataContext db = new NorthvilleLibraryDataContext(Properties.Settings.Default.NorthvilleLibraryV2ConnectionString);

        public MainWindow()
        {
            InitializeComponent();
        }

        private void login_btn_Click(object sender, RoutedEventArgs e)
        {
            if (tbUsername.Text == "" || tbPassword.Text == "")
            {
                MessageBox.Show("Please enter username and password");
                return;
            }

            else
            {
                if (comparingPassword(fetchingPassword()) == 0)
                {
                    MessageBox.Show("Login Successful", "Welcome Back!", MessageBoxButton.OK, MessageBoxImage.Exclamation);

                    var users = (from u in db.users
                                 where u.username == tbUsername.Text
                                 select u).FirstOrDefault();

                    if (users.user_role == "Librarian")
                    {
                        LibrarianAdminView librarianAdminView = new LibrarianAdminView();
                        librarianAdminView.Show();
                        this.Close();
                    }
                    else if (users.user_role == "Student")
                    {
                    }
                }
                else
                {
                    MessageBox.Show("Invalid username or password", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private string fetchingPassword()
        {
            string fetchedPassword = "";

            var users = (from u in db.users
                         where u.username == tbUsername.Text
                         select u).FirstOrDefault();

            if (users == null)
            {
                return "";
            }

            fetchedPassword = users.user_password;
            return fetchedPassword;
        }

        private int comparingPassword(string fetchedPassword)
        {
            if (tbPassword.Text == fetchedPassword)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }
    }
}
