using IntegrativeProgramming_UI.Helpers;
using IntegrativeProgramming_UI.Services;
using System;
using System.Collections.Generic;
using System.Data.Linq.SqlClient;
using System.Linq;
using System.Security.Policy;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;



namespace IntegrativeProgramming_UI
{
    /// <summary>
    /// Interaction logic for LibrarianAdminView.xaml
    /// </summary>
    public partial class LibrarianAdminView : Window
    {

        // Define constants for panel widths
        public readonly string _role = "Librarian";
        private string currentSection = "";
        private ViewReloader _viewReloader;

        #region Service Instances
        BookCopyService BookCopyService = null;
        AttendanceService AttendanceService = null;
        BorrowService BorrowService = null;
        FineService FineService = null;
        PaymentService PaymentService = null;
        CourseService CourseService = null;
        StudentService StudentService = null;
        UserService UserService = null;

        #endregion

        private MainActionsBuilder _actionBuilder;
        NorthvilleLibraryDataContext db = new NorthvilleLibraryDataContext(Properties.Settings.Default.NorthvilleLibraryV2ConnectionString);
        private RoutedEventHandler[] previousHandlers = new RoutedEventHandler[4];

        readonly string _username;
        private string currentViewKey = "";


        public LibrarianAdminView(string username)
        {
            InitializeComponent();
            _viewReloader = new ViewReloader();

            BookCopyService = new BookCopyService(db);
            BorrowService = new BorrowService(db);
            AttendanceService = new AttendanceService(db);
            FineService = new FineService(db);
            PaymentService = new PaymentService(db);
            CourseService = new CourseService(db);
            StudentService = new StudentService(db);
            UserService = new UserService(db);


            _actionBuilder = new MainActionsBuilder(this);
            LoadMainActions();
            LoadStatusCards();
            _username = username;
            tbUsername.Text = _username.Substring(3);
            RegisterViewReloaders();
        }

        #region Helpers and UI
        private void LoadStatusCards()
        {
            try
            {
               
                int totalVisits = db.vw_visit_logs.Count();
                int booksBorrowed = db.borrow_transactions.Count();
                int overdueBooks = db.borrow_transactions
                    .ToList() 
                    .Count(b => b.return_date == null &&
                                b.borrow_date.AddDays(7) < DateTime.Today);

                tbVisitCount.Text = totalVisits.ToString();
                tbBorrowCount.Text = booksBorrowed.ToString();
                tbOverdueCount.Text = overdueBooks.ToString();
            }
            catch (Exception ex)
            {
                MessageBoxBuilder.ShowError("Failed to load dashboard statistics.\n\nDetails: " + ex.Message);
            }
        }


    


        private void LoadMainActions()
        {
            var actions = _actionBuilder.GetActionsFor(currentSection);
            var buttons = new[] { btnAction1, btnAction2, btnAction3, btnAction4 };
            var labels = new[] { lblButtonAction1, lblButtonAction2, lblButtonAction3, lblButtonAction4 };

            for (int i = 0; i < buttons.Length; i++)
            {
                if (i < actions.Count)
                {
                    buttons[i].Visibility = Visibility.Visible;

                    if (previousHandlers[i] != null)
                        buttons[i].Click -= previousHandlers[i];

                    buttons[i].Click += actions[i].ClickHandler;
                    previousHandlers[i] = actions[i].ClickHandler;

                    var tb = buttons[i].FindName(labels[i].Name) as TextBlock;
                    if (tb != null)
                    {
                        tb.Text = actions[i].Label;
                        tb.Foreground = actions[i].HighlightColor;
                    }

                    buttons[i].BorderBrush = actions[i].HighlightColor;
                }
                else
                {
                    buttons[i].Visibility = Visibility.Collapsed;

                    if (previousHandlers[i] != null)
                        buttons[i].Click -= previousHandlers[i];

                    previousHandlers[i] = null;
                }
            }
        }

        private void IdentifyKey(string key)
        {
            currentViewKey = key;
            tbFormPanel.Text = $"{key} Form";
            tbTitle.Content = currentViewKey;
        }


        private void RegisterViewReloaders()
        {
            _viewReloader.Register("Borrow Transactions", LoadBorrowTransactions);
            _viewReloader.Register("Available Books", ShowAvailableBooks);
            _viewReloader.Register("Book Table", LoadBookTable);
            _viewReloader.Register("Overdue Books", LoadOverdueBooks);
            _viewReloader.Register("Fine Table", LoadFineTable);
            _viewReloader.Register("User Table", LoadUserTable);
            _viewReloader.Register("Course Table", LoadCourseTable);
            _viewReloader.Register("Payment Table", LoadPaymentTable);
            _viewReloader.Register("Attendance Table", LoadAttendanceTable);
            _viewReloader.Register("Borrowed Books", LoadBorrowedBooks);
        }
   

        #endregion
        private void OnEditClick(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var row = btn?.Tag;

            if (row == null) return;

            ShowFormPanel();
            try
            {
                CRUDHelper.EditEntityByRow(row, currentViewKey, spFormFields, db, () =>
                {
                    HideFormPanel(null, null);
                    _viewReloader.Reload(currentViewKey);
                });
            }
            catch (Exception ex)
            {
                MessageBoxBuilder.ShowError($"An unexpected error occurred while trying to edit the record.\n\nDetails: {ex.Message}", "Edit Error");
            }


        }



        private void OnDeleteClick(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var row = btn?.Tag;

            if (row == null) return;

            var confirm = MessageBox.Show("Are you sure you want to delete this record?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (confirm != MessageBoxResult.Yes) return;

            try
            {
                CRUDHelper.DeleteEntityByRow(row, currentViewKey, db);
            }
            catch (Exception ex)
            {
                MessageBoxBuilder.ShowError($"An unexpected error occurred while trying to delete the record.\n\nDetails: {ex.Message}", "Delete Error");
            }


            _viewReloader.Reload(currentViewKey);
        }



        #region MainActionHandlers

        // HOME
        public void OnBorrowClick(object sender, RoutedEventArgs e)
        {
            IdentifyKey("Available Books");
            ShowFormPanel();
            ShowAvailableBooks();

            try
            {
                BorrowService.HandleBorrow(spFormFields, () =>
                {
                    HideFormPanel(null, null);
                    _viewReloader.Reload(currentViewKey);
                   
                });
            }
            catch (Exception ex)
            {
                MessageBoxBuilder.ShowError($"An unexpected error occurred while trying to borrow the record.\n\nDetails: {ex.Message}", "Borrow Error");
                HideFormPanel(null, null);
            }


            ShowAvailableBooks();
        }

        public void LoadBorrowedBooks()
        {
            IdentifyKey("Borrowed Books");
            dgDataGrid.ItemsSource = BookCopyService.LoadBorrowedBooks();
        }
        public void OnReturnClick(object sender, RoutedEventArgs e)
        {
            ShowFormPanel();
            IdentifyKey("Borrowed Books");

            try
            {
                BorrowService.HandleReturn(spFormFields, () =>
                {
                    HideFormPanel(null, null);
                    _viewReloader.Reload(currentViewKey);
                });
            }
            catch (Exception ex)
            {
                MessageBoxBuilder.ShowError($"An unexpected error occurred while trying to return the record.\n\nDetails: {ex.Message}", "Return Error");
            }


            LoadBorrowTransactions();
        }

        public void LoadBorrowTransactions()
        {
            IdentifyKey("Borrow Transactions");
            dgDataGrid.ItemsSource = BorrowService.LoadBorrowTransactions();
        }

        public void LoadBorrowTransactions(object sender, RoutedEventArgs e)
        {
            IdentifyKey("Borrow Transactions");
            dgDataGrid.ItemsSource = BorrowService.LoadBorrowTransactions();
        }

        public void LoadAttendanceTable(object sender, RoutedEventArgs e)
        {
            dgDataGrid.ItemsSource = AttendanceService.LoadAttendanceTable();
        }
        public void LoadAttendanceTable()
        {
            dgDataGrid.ItemsSource = AttendanceService.LoadAttendanceTable();
        }
        public void OnAttendanceClick(object sender, RoutedEventArgs e)
        {
            ShowFormPanel();
            IdentifyKey("Attendance Table");
            LoadAttendanceTable();

            try
            {
                AttendanceService.CreateAttendance(spFormFields, () =>
                {
                    HideFormPanel(null, null);
                    _viewReloader.Reload(currentViewKey);
                    LoadStatusCards();
                });
            }
            catch (Exception ex)
            {
                MessageBoxBuilder.ShowError($"An unexpected error occurred while trying to add the record.\n\nDetails: {ex.Message}", "Create Error");
            }

        }

        private void LoadBookTable()
        {
            dgDataGrid.ItemsSource = BookCopyService.LoadBookCopyTable();
        }
        public void OnAddBookClick(object sender, RoutedEventArgs e)
        {
            ShowFormPanel();
            IdentifyKey("Book Table");
            LoadBookTable();

            try
            {
                BookCopyService.AddBook(spFormFields, () =>
                    {
                        HideFormPanel(null, null);
                        _viewReloader.Reload(currentViewKey);
                    });
            }
            catch (Exception ex)
            {
                MessageBoxBuilder.ShowError($"An unexpected error occurred while trying to add the book.\n\nDetails: {ex.Message}", "Create Book Error");
            }

        }

        private void LoadOverdueBooks()
        {
            dgDataGrid.ItemsSource = BookCopyService.LoadOverdueBooks();
        }
        public void OnOverdueClick(object sender, RoutedEventArgs e)
        {
            IdentifyKey("Overdue Books");
            LoadOverdueBooks();
        }

        private void ShowAvailableBooks()
        {
            IdentifyKey("Available Books");
            dgDataGrid.ItemsSource = BookCopyService.LoadAvailableBooks();
        }
        public void OnAvailableClick(object sender, RoutedEventArgs e)
        {
            IdentifyKey("Available Books");
            ShowAvailableBooks();

        }

        //TRANSACTOINS

        private void LoadPaymentTable()
        {
            dgDataGrid.ItemsSource = PaymentService.LoadPayment();
        }
        public void OnPaymentsClick(object sender, RoutedEventArgs e)
        {
            IdentifyKey("Payment Table");
            LoadPaymentTable();
        }

        private void LoadFineTable()
        {
            dgDataGrid.ItemsSource = FineService.LoadFineTable();
        }
        public void OnFinesClick(object sender, RoutedEventArgs e)
        {
            IdentifyKey("Fine Table");
            LoadFineTable();
        }

        //MANAGE

        private void LoadUserTable()
        {
            dgDataGrid.ItemsSource = UserService.LoadUserTable();
        }

        public void OnAddUserClick(object sender, RoutedEventArgs e)
        {
            ShowFormPanel();

            IdentifyKey("User Table");
            LoadUserTable();

            try
            {
                UserService.AddUser(spFormFields, () =>
                    {
                        HideFormPanel(null, null);
                        _viewReloader.Reload(currentViewKey);
                    });
            }
            catch (Exception ex)
            {
                MessageBoxBuilder.ShowError($"An unexpected error occurred while trying to add user.\n\nDetails: {ex.Message}", "Add User Error");
            }

        }


        private void LoadStudentTable()
        {
            IdentifyKey("Student Table");
            dgDataGrid.ItemsSource = StudentService.LoadStudentTable();
        }

        public void OnAddStudentClick(object sender, RoutedEventArgs e)
        {
            LoadStudentTable();
            ShowFormPanel();

            try
            {
                StudentService.AddStudent(spFormFields, () =>
                {
                    HideFormPanel(null, null);
                    _viewReloader.Reload(currentViewKey);
                });
            }
            catch (Exception ex)
            {
                MessageBoxBuilder.ShowError($"An unexpected error occurred while trying to add the student.\n\nDetails: {ex.Message}", "Add Student Error");
            }
            LoadStudentTable();

        }


        private void LoadCourseTable()
        {
            dgDataGrid.ItemsSource = CourseService.LoadCourseTable();
        }
        public void OnAddCourseClick(object sender, RoutedEventArgs e)
        {
            ShowFormPanel();

            IdentifyKey("Course Table");
            LoadCourseTable();

            try
            {
                CourseService.CreateCourse(spFormFields, () =>
                    {
                        HideFormPanel(null, null);
                        _viewReloader.Reload(currentViewKey);
                    });
            }
            catch (Exception ex)
            {
                MessageBoxBuilder.ShowError($"An unexpected error occurred while trying to add the course.\n\nDetails: {ex.Message}", "Add Course Error");
            }


        }


        #endregion
        #region SidePanels
        private void ToggleSidebar(object sender, RoutedEventArgs e)
        {

            Sidebar.Visibility = Visibility.Collapsed;
            CollapsedSidebar.Visibility = Visibility.Visible;
            SidebarColumn.Width = new GridLength(UIConstants.SidebarCollapsed);
        }

        private void ExpandSidebar(object sender, RoutedEventArgs e)
        {
            CollapsedSidebar.Visibility = Visibility.Collapsed;
            Sidebar.Visibility = Visibility.Visible;
            SidebarColumn.Width = new GridLength(UIConstants.SidebarExpanded);
        }

        private void ShowFormPanel()
        {

            FormPanel.Visibility = Visibility.Visible;
            FormPanelColumn.Width = new GridLength(UIConstants.FormPanelWidth);
        }

        private void HideFormPanel(object sender, RoutedEventArgs e)
        {
            FormPanelColumn.Width = new GridLength(0);
        }
        #endregion



        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            currentSection = "Home";
            tbPageTitle.Text = $"{currentSection}";
            LoadMainActions();
            HideFormPanel(null, null);
            LoadBorrowTransactions();
        }

        private void btnCatalog_Click(object sender, RoutedEventArgs e)
        {
            currentSection = "Catalog";
            tbPageTitle.Text = $"{currentSection}";
            LoadMainActions();
            LoadBookTable();
            HideFormPanel(null, null);
        }

        private void btnLogs_Click(object sender, RoutedEventArgs e)
        {
            currentSection = "Logs";
            tbPageTitle.Text = $"{currentSection}";
            HideFormPanel(null, null);
            LoadAttendanceTable();
            LoadMainActions();
        }

        private void btnManage_Click(object sender, RoutedEventArgs e)
        {
            currentSection = "Manage";
            tbPageTitle.Text = $"{currentSection}";
            IdentifyKey("User Table");
            HideFormPanel(null, null);
            LoadMainActions();
            LoadUserTable();
        }

        private void btnLogOut_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBoxBuilder.ShowConfirm("Are you sure you want to log out?", "Confirm Logout") == MessageBoxResult.Yes)
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
        }



    }
}

