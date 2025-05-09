using IntegrativeProgramming_UI.Models.Logs;
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
    internal class BorrowService
    {
        private readonly NorthvilleLibraryDataContext db = new NorthvilleLibraryDataContext(Properties.Settings.Default.NorthvilleLibraryV2ConnectionString);

        public BorrowService(NorthvilleLibraryDataContext _db)
        {
            this.db = _db;    
        }
        public ObservableCollection<BorrowTransactionDisplay> LoadBorrowTransactions()
        {
            return new ObservableCollection<BorrowTransactionDisplay>(
                from bt in db.borrow_transactions
                select new BorrowTransactionDisplay
                {
                    BorrowID = bt.borrow_id,
                    StudentID = bt.student_id,
                    CopyID = bt.book_copy_id,
                    BorrowDate = bt.borrow_date,
                    ReturnDate = bt.return_date,
                    Status = bt.borrow_status,
                    StudentName = bt.student.student_name,
                    BookTitle = bt.book_copy.book_edition.book.book_title
                });
        }


        public void HandleBorrow(StackPanel sp, Action onSuccess)
        {
            FormBuilder.BuildBorrowForm(sp,db,onSuccess); 
        }

        public void HandleReturn(StackPanel sp, Action onSuccess) 
        {
            FormBuilder.BuildReturnForm(sp,db,onSuccess);
        }

        public void DeleteBorrowCascade(string borrowId)
        {
            var fines = db.fines.Where(f => f.borrow_id == borrowId).ToList();
            foreach (var fine in fines)
            {
                var payments = db.payments.Where(p => p.fine_id == fine.fine_id);
                db.payments.DeleteAllOnSubmit(payments);
                try
                {
                    db.fines.DeleteOnSubmit(fine);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Delete failed: " + ex.Message);
                }
            }

            var borrow = db.borrow_transactions.FirstOrDefault(b => b.borrow_id == borrowId);
            if (borrow != null)
            {
                db.borrow_transactions.DeleteOnSubmit(borrow);
                try
                {
                    db.SubmitChanges();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Delete failed: " + ex.Message);
                }
            }
        }

    }
}
