using IntegrativeProgramming_UI.Helpers;
using IntegrativeProgramming_UI.Services;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;

namespace IntegrativeProgramming_UI
{
    /// <summary>
    /// Interaction logic for StudentView.xaml
    /// </summary>
    public partial class StudentView : Window
    {
        public readonly string _role = "Student";
        private string studentID;
   
        BookCopyService BookCopyService = null;
        AttendanceService AttendanceService = null;
        BorrowService BorrowService = null;
        StudentService StudentService = null;

        NorthvilleLibraryDataContext db = new NorthvilleLibraryDataContext(Properties.Settings.Default.NorthvilleLibraryV2ConnectionString);
        private RoutedEventHandler[] previousHandlers = new RoutedEventHandler[4];

        public StudentView(string userName)
        {
            InitializeComponent();
            studentID = userName;
            BorrowService = new BorrowService(db);
            BookCopyService = new BookCopyService(db);
            AttendanceService = new AttendanceService(db);
            StudentService = new StudentService(db);

            string name = (from ut in db.students
                                  where ut.student_id == studentID
                                  select ut.student_name).FirstOrDefault();

            tbStudentName.Text = name;
            tbWelcomeName.Text = name;

            LoadPreviousActivity();
            LoadStatusCards();
        }

        private void LoadStatusCards()
        {
            try
            {
            
                int totalBorrowed = db.borrow_transactions
                    .Where(b => b.student_id == studentID)
                    .Count();

             
                var borrowedItems = db.borrow_transactions
                    .Where(b => b.student_id == studentID && b.return_date == null)
                    .ToList();

                int overdueCount = borrowedItems
                    .Count(b => b.borrow_date.AddDays(7) < DateTime.Now);

                int totalVisits = db.vw_visit_logs
                    .Where(v => v.student_id == studentID)
                    .Count();

             
                txtBooksBorrowed.Text = totalBorrowed.ToString();
                txtOverdueItems.Text = overdueCount.ToString();
                txtLibraryVisits.Text = totalVisits.ToString();
            }
            catch (Exception ex)
            {
                MessageBoxBuilder.ShowError($"Failed to load statistics.\n\nDetails: {ex.Message}", "Dashboard Error");
            }
        }


        private void LoadPreviousActivity()
        {
            try
            {
                var activeBorrows = db.borrow_transactions
                    .Where(b => b.student_id == studentID && b.borrow_status == "Borrowed") // Adjust status value as needed
                    .OrderByDescending(b => b.borrow_date)
                    .ToList();

                var formattedHistory = activeBorrows.Select(b => new
                {
                    CopyID = b.book_copy_id,
                    BorrowDate = b.borrow_date.ToString("MM/dd/yyyy"),
                    Expected_ReturnDate = b.borrow_date.AddDays(7).ToString("MM/dd/yyyy"),
                    Status = b.borrow_status,
                    BookTitle = b.book_copy.book_edition.book.book_title
                }).ToList();

                dgDataGrid.ItemsSource = formattedHistory;
            }
            catch (System.Exception ex)
            {
                MessageBoxBuilder.ShowError($"Failed to load active borrow records.\n\nDetails: {ex.Message}", "Load Error");
            }
        }




        private void btnMyRecords_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var rawBorrowHistory = db.borrow_transactions
                    .Where(b => b.student_id == studentID)
                    .OrderByDescending(b => b.borrow_date)
                    .ToList(); // Materialize first before formatting

                var borrowHistory = rawBorrowHistory.Select(b => new
                {
                    BookTitle = b.book_copy.book_edition.book.book_title,
                    BorrowDate = b.borrow_date.ToString("MM/dd/yyyy"),
                    DueDate = b.borrow_date.AddDays(7).ToString("MM/dd/yyyy"),
                    ReturnDate = b.return_date.HasValue ? b.return_date.Value.ToString("MM/dd/yyyy") : "Not Returned",
                    Status = b.borrow_status
                }).ToList();

                dgDataGrid.ItemsSource = borrowHistory;
            }
            catch (System.Exception ex)
            {
                MessageBoxBuilder.ShowError($"Unable to load your borrow records.\n\nDetails: {ex.Message}", "Error");
            }
        }


        private void btnAttendance_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var rawLogs = db.vw_visit_logs
                    .Where(a => a.student_id == studentID)
                    .OrderByDescending(a => a.date_of_visit)
                    .ToList();

                var attendanceLogs = rawLogs.Select(a => new
                {
                    DateVisited = a.date_of_visit.ToString("MM/dd/yyyy"),
                    TimeVisited = a.time_of_visit.ToString(@"hh\:mm\:ss") ?? "N/A"
                }).ToList();

                // Temporary simulated attendance log
                var simulatedEntry = new
                {
                    DateVisited = DateTime.Now.ToString("MM/dd/yyyy"),
                    TimeVisited = DateTime.Now.ToString("HH:mm:ss")
                };

                // Add the simulated entry to the top
                attendanceLogs.Insert(0, simulatedEntry);

                // Bind to DataGrid
                dgDataGrid.ItemsSource = attendanceLogs;

               
            }
            catch (System.Exception ex)
            {
                MessageBoxBuilder.ShowError($"Unable to load your attendance logs.\n\nDetails: {ex.Message}", "Error");
            }
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
               "Are you sure you want to log out?",
               "Confirm Logout",
               MessageBoxButton.YesNo,
               MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
        }
    }
}

