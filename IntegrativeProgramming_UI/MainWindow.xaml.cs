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

      
        private void ThemeToggle_Checked(object sender, RoutedEventArgs e)
        {
            // Switch to Dark Theme
            Resources["BackgroundBrush"] = new SolidColorBrush((Color)Resources["DarkBackground"]);
            Resources["PanelBrush"] = new SolidColorBrush((Color)Resources["DarkPanel"]);
            Resources["TextBrush"] = new SolidColorBrush((Color)Resources["DarkText"]);
            Resources["AccentBrush"] = new SolidColorBrush((Color)Resources["DarkAccent"]);
            Resources["SecondaryBrush"] = new SolidColorBrush((Color)Resources["DarkSecondary"]);
        }

        private void ThemeToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            // Switch to Light Theme
            Resources["BackgroundBrush"] = new SolidColorBrush((Color)Resources["LightBackground"]);
            Resources["PanelBrush"] = new SolidColorBrush((Color)Resources["LightPanel"]);
            Resources["TextBrush"] = new SolidColorBrush((Color)Resources["LightText"]);
            Resources["AccentBrush"] = new SolidColorBrush((Color)Resources["LightAccent"]);
            Resources["SecondaryBrush"] = new SolidColorBrush((Color)Resources["LightSecondary"]);
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

        private void btnLogin_Click(object sender, RoutedEventArgs e)
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
                        LibrarianAdminView librarianAdminView = new LibrarianAdminView(tbUsername.Text);
                        librarianAdminView.Show();
                        this.Close();
                    }
                    else if (users.user_role == "Clerical Assistant")
                    {
                        ClericalAssistantView clericalAssistantView = new ClericalAssistantView();
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
                    MessageBox.Show("Invalid username or password", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }
    }
}
