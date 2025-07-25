﻿using IntegrativeProgramming_UI.Helpers;
using IntegrativeProgramming_UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;



namespace IntegrativeProgramming_UI
{
    /// <summary>
    /// Interaction logic for LibrarianAdminView.xaml
    /// </summary>
    public partial class ClericalAssistantView : Window
    {

        // Define constants for panel widths
        private string currentSection = "";
        private ViewReloader _viewReloader;
        public readonly string _role = "Clerical Assistant";

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


        public ClericalAssistantView(string username)
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
            LoadStatusCards();
            _username = username;
            lblWelcomeUser.Text = _username;
        }


        private void RegisterViewReloaders()
        {
            _viewReloader.Register("Borrow Transactions", LoadBorrowTransactions);
            _viewReloader.Register("Available Books", ShowAvailableBooks);
            _viewReloader.Register("Book Table", LoadBookTable);
            _viewReloader.Register("Overdue Books", LoadOverdueBooks);
            _viewReloader.Register("Fine Table", LoadFineTable);
            _viewReloader.Register("Course Table", LoadCourseTable);
            _viewReloader.Register("Payment Table", LoadPaymentTable);
            _viewReloader.Register("Attendance Table", LoadAttendanceTable);
        }

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

            try
            {
                BorrowService.HandleBorrow(spFormFields, () =>
                {
                    HideFormPanel(null, null);
                    _viewReloader.Reload(currentViewKey);
                    LoadStatusCards();
                });
                ShowAvailableBooks();
            }
            catch (Exception ex)
            {
                MessageBoxBuilder.ShowError("Failed to initiate borrowing.\n\nDetails: " + ex.Message, "Borrow Error");
            }

            ShowAvailableBooks();
        }
        public void OnReturnClick(object sender, RoutedEventArgs e)
        {
            ShowFormPanel();
            IdentifyKey("Borrow Transactions");

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
                MessageBoxBuilder.ShowError("Failed to return item.\n\nDetails: " + ex.Message, "Return Error");
            }
        }

        public void LoadBorrowTransactions()
        {
            try
            {
                IdentifyKey("Borrow Transactions");
                dgDataGrid.ItemsSource = BorrowService.LoadBorrowTransactions();
            }
            catch (Exception ex)
            {
                MessageBoxBuilder.ShowError("Failed to load borrow transactions.\n\nDetails: " + ex.Message, "Load Error");
            }
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
                MessageBoxBuilder.ShowError("Failed to mark attendance.\n\nDetails: " + ex.Message, "Attendance Error");
            }
        }
        private void LoadBookTable()
        {
            dgDataGrid.ItemsSource = BookCopyService.LoadBookCopyTable();
        }

        private void LoadOverdueBooks()
        {
            dgDataGrid.ItemsSource = BookCopyService.LoadOverdueBooks();
        }
        public void OnOverdueClick(object sender, RoutedEventArgs e)
        {
            IdentifyKey("Book Table");
            LoadOverdueBooks();
        }

        private void ShowAvailableBooks()
        {
            IdentifyKey("Book Table");
            dgDataGrid.ItemsSource = BookCopyService.LoadAvailableBooks();
        }
        public void OnAvailableClick(object sender, RoutedEventArgs e)
        {
            ShowAvailableBooks();
        }

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

     
        public void LoadStudentTable(object sender, RoutedEventArgs e)
        {
            IdentifyKey("Student Table");
            dgDataGrid.ItemsSource = StudentService.LoadStudentTable();
        }

        public void LoadCourseTable()
        {
            IdentifyKey("Course Table");
            dgDataGrid.ItemsSource = CourseService.LoadCourseTable();
        }



        public void LoadCourseTable(object sender, RoutedEventArgs e)
        {
            IdentifyKey("Course Table");
            dgDataGrid.ItemsSource = CourseService.LoadCourseTable();
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
            tbPageTitle.Content = $"{currentSection}";
            LoadMainActions();
            HideFormPanel(null, null);
            LoadBorrowTransactions();
        }

        private void btnCatalog_Click(object sender, RoutedEventArgs e)
        {
            currentSection = "Catalog";
            tbPageTitle.Content = $"{currentSection}";
            LoadMainActions();
            LoadBookTable();
            HideFormPanel(null, null);
        }

        private void btnLogs_Click(object sender, RoutedEventArgs e)
        {
            currentSection = "Logs";
            tbPageTitle.Content = $"{currentSection}";
            HideFormPanel(null, null);
            LoadAttendanceTable();
            LoadMainActions();
        }

        private void btnManage_Click(object sender, RoutedEventArgs e)
        {
            currentSection = "Manage";
            tbPageTitle.Content = $"{currentSection}";
            IdentifyKey("User Table");
            HideFormPanel(null, null);
            LoadMainActions();
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

