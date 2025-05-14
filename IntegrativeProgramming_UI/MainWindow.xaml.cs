using IntegrativeProgramming_UI.Helpers;
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
            if (tbPassword.Password == fetchedPassword)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbUsername.Text) || string.IsNullOrWhiteSpace(tbPassword.Password))
            {
                MessageBoxBuilder.ShowIncompleteInput("Username and Password");
                return;
            }

            if (comparingPassword(fetchingPassword()) == 0)
            {
                MessageBoxBuilder.ShowSuccess("Login Successful", "Welcome Back!");

                var users = (from u in db.users
                             where u.username == tbUsername.Text
                             select u).FirstOrDefault();

                if (users != null)
                {
                    if (users.user_role == "Librarian")
                    {
                        LibrarianAdminView librarianAdminView = new LibrarianAdminView(tbUsername.Text);
                        librarianAdminView.Show();
                        this.Close();
                    }
                    else if (users.user_role == "Clerical Assistant")
                    {
                        ClericalAssistantView clericalAssistantView = new ClericalAssistantView(tbUsername.Text);
                        clericalAssistantView.Show();
                        this.Close();
                    }
                    else if (users.user_role == "Student")
                    {
                        StudentView studentView = new StudentView(users.username);
                        studentView.Show();
                        this.Close();
                    }
                }
                else
                {
                    MessageBoxBuilder.ShowNotFound("User");
                }
            }
            else
            {
                MessageBoxBuilder.ShowError("Invalid username or password", "Login Failed");
            }
        }

    }
}
