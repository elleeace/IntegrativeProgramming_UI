using IntegrativeProgramming_UI.Helpers;
using IntegrativeProgramming_UI.Services.LogServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;

namespace IntegrativeProgramming_UI.Services
{
    internal class FineService
    {
        private readonly NorthvilleLibraryDataContext db = new NorthvilleLibraryDataContext(Properties.Settings.Default.NorthvilleLibraryV2ConnectionString);
        public FineService(NorthvilleLibraryDataContext db)
        {
            this.db = db;
        }

        public ObservableCollection<FineDisplay> LoadFineTable()
        {
            return new ObservableCollection<FineDisplay>(
                (from ft in db.vw_fines_reports
                 select new FineDisplay
                 {
                     FineID = ft.fine_id,
                     BorrowID = ft.borrow_id,
                     StudentID = ft.student_id,
                     PaymentAmount = ft.payment_amount ?? 0,
                     AssessedDate = ft.assessed_date,
                     Paid = ft.is_paid
                 })
            );
        }

        public void DeleteFine(string fineId)
        {
            var fine = db.fines.FirstOrDefault(f => f.fine_id == fineId);
            if (fine != null)
            {
                db.fines.DeleteOnSubmit(fine);
                CRUDHelper.SafeSubmit(db);
            }
        }

    }


}

