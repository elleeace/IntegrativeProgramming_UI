using System.Collections.Generic;
using System.Windows.Media;

namespace IntegrativeProgramming_UI
{
    internal class MainActionsBuilder
    {
        private readonly LibrarianAdminView _view;
        private readonly ClericalAssistantView _assistantView;
        private readonly string _userRole;


        public MainActionsBuilder(LibrarianAdminView view)
        {
            _view = view;
            _userRole = view._role;
        }

        public MainActionsBuilder(ClericalAssistantView v)
        {
            _assistantView = v;
            _userRole = v._role;
        }

        public List<MainAction> GetActionsFor(string section)
        {
            if (_userRole == "Clerical Assistant")
            {
                switch (section)
                {
                    case "Home":
                        return new List<MainAction>
                {
                    new MainAction { Label = "Borrow Book", ClickHandler = _assistantView.OnBorrowClick, HighlightColor = Brushes.SlateBlue },
                    new MainAction { Label = "Return Book", ClickHandler = _assistantView.OnReturnClick, HighlightColor = Brushes.MediumSeaGreen },
                    new MainAction { Label = "Record Attendance", ClickHandler = _assistantView.OnAttendanceClick, HighlightColor = Brushes.LightSkyBlue }
                };

                    case "Catalog":
                        return new List<MainAction>
                {
                    new MainAction { Label = "Show Overdue", ClickHandler = _assistantView.OnOverdueClick, HighlightColor = Brushes.Orange },
                    new MainAction { Label = "Show Available", ClickHandler = _assistantView.OnAvailableClick, HighlightColor = Brushes.LightSkyBlue }
                };

                    case "Logs":
                        return new List<MainAction>
                {
                    new MainAction { Label = "View Borrows", ClickHandler = _assistantView.LoadBorrowTransactions, HighlightColor = Brushes.SlateBlue },
                    new MainAction { Label = "View Attendance", ClickHandler = _assistantView.LoadAttendanceTable, HighlightColor = Brushes.LightSkyBlue },
                    new MainAction { Label = "View Fines", ClickHandler = _assistantView.OnFinesClick, HighlightColor = Brushes.Orange },
                      new MainAction { Label = "View Payment", ClickHandler = _assistantView.OnPaymentsClick, HighlightColor = Brushes.MediumSeaGreen}
                };

                    case "Manage":

                        return new List<MainAction>
                        {     new MainAction { Label = "View Students", ClickHandler = _assistantView.LoadStudentTable, HighlightColor = Brushes.Orange },
                new MainAction { Label = "View Courses", ClickHandler = _assistantView.LoadCourseTable, HighlightColor = Brushes.MediumSeaGreen }
                        };

                    default:
                        return new List<MainAction>();
                }
            }

            // Default: Librarian role
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
