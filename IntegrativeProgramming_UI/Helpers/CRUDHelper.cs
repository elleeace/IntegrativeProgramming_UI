using IntegrativeProgramming_UI.Models.Catalog;
using IntegrativeProgramming_UI.Models.Logs;
using IntegrativeProgramming_UI.Models;
using IntegrativeProgramming_UI.Services;
using IntegrativeProgramming_UI.Services.LogServices;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace IntegrativeProgramming_UI.Helpers
{
    internal static class CRUDHelper
    {
        public static void SafeSubmit(NorthvilleLibraryDataContext db)
        {
            try
            {
                db.SubmitChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database error: " + ex.Message);
            }
        }

        public static void DeleteEntityByRow(object row, string currentViewKey, NorthvilleLibraryDataContext db)
        {
            switch (row)
            {
                case BorrowTransactionDisplay borrow:
                    new BorrowService(db).DeleteBorrowCascade(borrow.BorrowID);
                    break;

                case BookCopyDisplay bookCopy:
                    new BookCopyService(db).DeleteCopyCascade(bookCopy.CopyID);
                    break;

                case User user:
                    new UserService(db).DeleteUser(user.UserID);
                    break;

                case Student student:
                    new StudentService(db).DeleteStudent(student.StudentID);
                    break;

                case Course course:
                    new CourseService(db).DeleteCourse(course.CourseID);
                    break;

                case FineDisplay fine when currentViewKey == "Fine Table":
                    new FineService(db).DeleteFine(fine.FineID);
                    break;

                case PaymentModel payment when currentViewKey == "Payment Table":
                    new PaymentService(db).DeletePayment(payment.PaymentID);
                    break;

                default:
                    MessageBox.Show("No matching handler found for deletion.");
                    break;
            }
        }

        public static void EditEntityByRow(object row, string currentViewKey, StackPanel formPanel, NorthvilleLibraryDataContext db, Action onSaved)
        {
            switch (row)
            {
                case User user:
                    new UserService(db).EditUser(formPanel, db, user, onSaved);
                    break;

                case Student student:
                    new StudentService(db).EditStudent(formPanel, db, student, onSaved);
                    break;

                case Course course:
                    new CourseService(db).EditCourse(formPanel, db, course, onSaved);
                    break;

                case BorrowTransactionDisplay borrowTransaction:
                    new BorrowService(db).EditBorrow(formPanel, db, borrowTransaction, onSaved);
                    break;
                case BookCopyDisplay copy:
                    new BookCopyService(db).EditBookCopy(formPanel, db, copy, onSaved);
                    break;
                default:
                    MessageBox.Show($"No matching handler found for editing. Object {row.ToString()}");
                    break;
            }
        }

    }
}
