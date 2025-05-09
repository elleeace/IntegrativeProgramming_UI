using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace IntegrativeProgramming_UI
{
    internal class MainActionsBuilder
    {
        private readonly LibrarianAdminView _view;

        public MainActionsBuilder(LibrarianAdminView view)
        {
            _view = view;
        }

        public List<MainAction> GetActionsFor(string section)
        {
            switch (section)
            {
                case "Home":
                    return new List<MainAction>
            {
                new MainAction { Label = "Borrow Book", ClickHandler = _view.OnBorrowClick, HighlightColor = Brushes.SlateBlue },
                new MainAction { Label = "Return Book", ClickHandler = _view.OnReturnClick, HighlightColor = Brushes.MediumSeaGreen },
                new MainAction { Label = "View Borrows", ClickHandler = _view.LoadBorrowTransactions, HighlightColor = Brushes.Orange },
                new MainAction { Label = "Record Attendance", ClickHandler = _view.OnAttendanceClick, HighlightColor = Brushes.LightSkyBlue }
            };

                case "Catalog":
                    return new List<MainAction>
            {
                new MainAction { Label = "Add Book", ClickHandler = _view.OnAddBookClick, HighlightColor = Brushes.MediumSeaGreen },
                new MainAction { Label = "Show Overdue", ClickHandler = _view.OnOverdueClick, HighlightColor = Brushes.Orange },
                new MainAction { Label = "Show Available", ClickHandler = _view.OnAvailableClick, HighlightColor = Brushes.LightSkyBlue }
            };

                case "Logs":
                    return new List<MainAction>
            {
                new MainAction { Label = "View Borrows", ClickHandler = _view.LoadBorrowTransactions, HighlightColor = Brushes.SlateBlue },
                new MainAction { Label = "View Payments", ClickHandler = _view.OnPaymentsClick, HighlightColor = Brushes.MediumSeaGreen },
                new MainAction { Label = "View Fines", ClickHandler = _view.OnFinesClick, HighlightColor = Brushes.Orange },
                 new MainAction { Label = "View Attendance", ClickHandler = _view.LoadAttendanceTable, HighlightColor = Brushes.LightSkyBlue }
            };

                case "Manage":
                    return new List<MainAction>
            {

                new MainAction { Label = "Add User", ClickHandler = _view.OnAddUserClick, HighlightColor = Brushes.SlateBlue },
                 new MainAction { Label = "Add Student", ClickHandler = _view.OnAddStudentClick, HighlightColor = Brushes.Orange },
                new MainAction { Label = "Add Course", ClickHandler = _view.OnAddCourseClick, HighlightColor = Brushes.MediumSeaGreen }
            };

                default:
                    return new List<MainAction>();
            }
        }

    }
}
