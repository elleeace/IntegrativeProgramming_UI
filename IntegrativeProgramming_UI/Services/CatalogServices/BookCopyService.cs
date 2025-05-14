using IntegrativeProgramming_UI.Helpers;
using IntegrativeProgramming_UI.Models;
using IntegrativeProgramming_UI.Models.Catalog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace IntegrativeProgramming_UI.Services
{
    internal class BookCopyService
    {
        private readonly NorthvilleLibraryDataContext db = new NorthvilleLibraryDataContext(Properties.Settings.Default.NorthvilleLibraryV2ConnectionString);

        public BookCopyService(NorthvilleLibraryDataContext _db)
        {
            db = _db;
        }

        public ObservableCollection<BookCopyDisplay> LoadBookTable()
        {
            return new ObservableCollection<BookCopyDisplay>(
                (from copy in db.book_copies
                 select new BookCopyDisplay
                 {
                     CopyID = copy.book_copy_id,
                     Title = copy.book_edition.book.book_title,
                     Author = copy.book_edition.book.book_author,
                     Genre = copy.book_edition.book.book_genre.genre_name,
                     ISBN = copy.book_edition.book_isbn,
                     Edition = copy.book_edition.edition_number.ToString(),
                     Format = copy.book_edition.book_format,
                     Status = copy.book_status.status_description,
                     Location = copy.book_location.room_name + ", " + copy.book_location.section_name
                 }).ToList());
        }

        public IEnumerable<object> LoadBorrowedBooks()
        {
            return (from bt in db.borrow_transactions
                    where bt.borrow_status == "Borrowed"
                    select new
                    {
                        BorrowID = bt.borrow_id,
                        StudentID = bt.student_id,
                        StudentName = bt.student.student_name,
                        CopyID = bt.book_copy_id,
                        BookTitle = bt.book_copy.book_edition.book.book_title,
                        BorrowDate = bt.borrow_date,
                        Location = bt.book_copy.book_location.room_name + ", " + bt.book_copy.book_location.section_name
                    }).ToList();
        }


        public IEnumerable<object> LoadOverdueBooks()
        {
            return (from ot in db.vw_OverdueBooks
                    select new
                    {
                        OverdueID = ot.borrow_id,
                        StudentID = ot.student_id,
                        StudentName = ot.student_name,
                        BorrowDate = ot.borrow_date,
                        DueDate = ot.due_date,
                        FineAmnt = ot.fine_amount,
                        CopyID = ot.book_copy_id,
                        BookTitle = ot.book_title,
                        Location = ot.room_name + ", " + ot.shelf_code + "," + ot.section_name
                    }).ToList();
        }

        public IEnumerable<object> LoadAvailableBooks()
        {
            return (from ab in db.vw_AvailableBooks
                    select new
                    {
                        CopyID = ab.book_copy_id,
                        Title = ab.book_title,
                        Genre = ab.genre_name,
                        Author = ab.book_author,
                        Status = "Availale",
                        Location = ab.room_name + ", " + ab.shelf_code + "," + ab.section_name
                    });
        }

        public void AddBook(StackPanel sp, Action onSucess)
        {
            FormBuilder.ShowAddBookForm(sp, db, onSucess);
        }

        public void EditBookCopy(StackPanel formPanel, NorthvilleLibraryDataContext db, BookCopyDisplay copy, Action onSaved)
        {
            FormBuilder.BuildEditBookCopyForm(formPanel, db, copy.CopyID, onSaved);
        }


        public void DeleteCopyCascade(string copyId)
        {
            var borrows = db.borrow_transactions.Where(b => b.book_copy_id == copyId).ToList();
            foreach (var borrow in borrows)
            {
                var fine = db.fines.FirstOrDefault(f => f.borrow_id == borrow.borrow_id);
                if (fine != null)
                {
                    var payments = db.payments.Where(p => p.fine_id == fine.fine_id);
                    db.payments.DeleteAllOnSubmit(payments);
                    db.fines.DeleteOnSubmit(fine);
                }
                db.borrow_transactions.DeleteOnSubmit(borrow);
            }

            var copy = db.book_copies.FirstOrDefault(c => c.book_copy_id == copyId);
            if (copy != null)
            {
                db.book_copies.DeleteOnSubmit(copy);
                try
                {
                    db.SubmitChanges();
                }
                catch (Exception ex)
                {
                    MessageBoxBuilder.ShowError("Delete failed.\n\nDetails: " + ex.Message);
                }
            }
        }
    }
}
