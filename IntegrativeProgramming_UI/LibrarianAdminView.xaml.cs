using IntegrativeProgramming_UI.Helpers;
using IntegrativeProgramming_UI.Services;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using IntegrativeProgramming_UI.Models.Catalog;
using IntegrativeProgramming_UI.Models.Logs;



namespace IntegrativeProgramming_UI
{
    /// <summary>
    /// Interaction logic for LibrarianAdminView.xaml
    /// </summary>
    public partial class LibrarianAdminView : Window
    {

        // Define constants for panel widths
        private string currentSection = "";
        private Dictionary<string, Action<object>> deleteHandlers;
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

        private string currentViewKey = "";


        public LibrarianAdminView()
        {
            InitializeComponent();
            _viewReloader = new ViewReloader();
            RegisterViewReloaders();

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
            btnSave.Visibility = Visibility.Collapsed;
        }

        private string GetValue(object obj, string propertyName)
        {
            return obj.GetType().GetProperty(propertyName)?.GetValue(obj)?.ToString();
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
        }


        private void DummyClick(object sender, RoutedEventArgs e) { }


        private void OnEditClick(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var row = btn?.Tag;

            if (row == null) return;

            ShowFormPanel();
            CRUDHelper.EditEntityByRow(row, currentViewKey, spFormFields, db, () =>
            {
                HideFormPanel(null, null);
                _viewReloader.Reload(currentViewKey);
            });

            btnSave.Visibility = Visibility.Visible;
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
                MessageBox.Show("Error: " + ex.Message);
            }

            _viewReloader.Reload(currentViewKey);
        }



        private void IdentifyKey(string key)
        {
            tbTitle.Content = key;
            currentViewKey = key;
        }


        #region MainActionHandlers

        // HOME
        public void OnBorrowClick(object sender, RoutedEventArgs e)
        {
            IdentifyKey("Available Books");
            ShowFormPanel();
            ShowAvailableBooks();
            BorrowService.HandleBorrow(spFormFields, () =>
            {
                HideFormPanel(null, null);
                _viewReloader.Reload(currentViewKey);
            });

            ShowAvailableBooks();
        }

        public void OnReturnClick(object sender, RoutedEventArgs e)
        {
            ShowFormPanel();
            IdentifyKey("Borrow Transactions");
            BorrowService.HandleReturn(spFormFields, () =>
            {
                HideFormPanel(null, null);
                _viewReloader.Reload(currentViewKey);
            });
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
            AttendanceService.CreateAttendance(spFormFields, () =>
                {
                    HideFormPanel(null, null);
                    _viewReloader.Reload(currentViewKey);
                });

        }

        private void LoadBookTable()
        {
            dgDataGrid.ItemsSource = BookCopyService.LoadBookTable();
        }
        public void OnAddBookClick(object sender, RoutedEventArgs e)
        {
            ShowFormPanel();
            IdentifyKey("Book Table");
            LoadBookTable();

            BookCopyService.AddBook(spFormFields, () =>
                {
                    HideFormPanel(null, null);
                    _viewReloader.Reload(currentViewKey);
                });
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

            UserService.AddUser(spFormFields, () =>
                {
                    HideFormPanel(null, null);
                    MessageBox.Show("User successfully added!");
                    _viewReloader.Reload(currentViewKey);
                });
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

            StudentService.AddStudent(spFormFields, () =>
            {
                MessageBox.Show("Student Successfully Added");
                _viewReloader.Reload(currentViewKey);
            });
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

            CourseService.CreateCourse(spFormFields, () =>
                {
                    HideFormPanel(null, null);
                    MessageBox.Show("Course successfully added!");
                    _viewReloader.Reload(currentViewKey);
                });

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

                    // Attach new handler
                    buttons[i].Click += actions[i].ClickHandler;
                    previousHandlers[i] = actions[i].ClickHandler;

                    // Update label inside the button
                    var tb = buttons[i].FindName(labels[i].Name) as TextBlock;
                    if (tb != null)
                    {
                        tb.Text = actions[i].Label;
                        tb.Foreground = actions[i].HighlightColor;
                    }

                    // Sync button border color with text
                    buttons[i].BorderBrush = actions[i].HighlightColor;
                }
                else
                {
                    buttons[i].Visibility = Visibility.Collapsed;

                    // Detach if it had a previous handler
                    if (previousHandlers[i] != null)
                        buttons[i].Click -= previousHandlers[i];

                    previousHandlers[i] = null;
                }
            }
        }


        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            currentSection = "Home";
            LoadMainActions();
            LoadBorrowTransactions();
        }

        private void btnCatalog_Click(object sender, RoutedEventArgs e)
        {
            currentSection = "Catalog";
            IdentifyKey("Book Table");
            LoadMainActions();
            dgDataGrid.ItemsSource = BookCopyService.LoadBookTable();
        }

        private void btnLogs_Click(object sender, RoutedEventArgs e)
        {
            currentSection = "Logs";
            LoadMainActions();
        }

        private void btnManage_Click(object sender, RoutedEventArgs e)
        {
            currentSection = "Manage";
            IdentifyKey("User Table");
            LoadMainActions();
            dgDataGrid.ItemsSource = UserService.LoadUserTable();
        }
    }
}

