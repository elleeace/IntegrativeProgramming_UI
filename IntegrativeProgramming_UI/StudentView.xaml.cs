using IntegrativeProgramming_UI.Services;
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
        private string currentSection = "";
        private ViewReloader _viewReloader;


        BookCopyService BookCopyService = null;
        AttendanceService AttendanceService = null;
        BorrowService BorrowService = null;
        StudentService StudentService = null;


        private MainActionsBuilder _actionBuilder;
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
        }

        private void LoadPreviousActivity()
        {
            var rawHistory = db.borrow_transactions
                .Where(b => b.student_id == studentID)
                .OrderByDescending(b => b.borrow_date)
                .ToList(); // force query execution in SQL first

            var formattedHistory = rawHistory.Select(b => new
            {
                CopyID = b.book_copy_id,
                BorrowDate = b.borrow_date.ToString("MM/dd/yyyy"),
                Expected_ReturnDate = b.borrow_date.AddDays(7).ToString("MM/dd/yyyy"),
                Status = b.borrow_status,
                BookTitle = b.book_copy.book_edition.book.book_title
            }).ToList();

            dgDataGrid.ItemsSource = formattedHistory;
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new MainWindow();
            loginWindow.Show();
            this.Close();
        }

        private void btnMyRecords_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnAttendance_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
