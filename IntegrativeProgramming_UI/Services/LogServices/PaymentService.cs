using IntegrativeProgramming_UI.Helpers;
using IntegrativeProgramming_UI.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace IntegrativeProgramming_UI.Services
{
    internal class PaymentService
    {
        private readonly NorthvilleLibraryDataContext db = new NorthvilleLibraryDataContext(Properties.Settings.Default.NorthvilleLibraryV2ConnectionString);

        public PaymentService(NorthvilleLibraryDataContext db)
        {
            this.db = db;
        }

        public ObservableCollection<PaymentModel> LoadPayment()
        {
            return new ObservableCollection<PaymentModel>(
                (from pt in db.payments
                 select new PaymentModel
                 {
                     PaymentID = pt.payment_id,
                     FineID = pt.fine_id,
                     PaymentDesc = pt.payment_desc,
                     PaymentDate = pt.payment_date,
                     Amount = pt.payment_amount 
                 })
            );
        }


        public void DeletePayment(string paymentId)
        {
            if (string.IsNullOrWhiteSpace(paymentId)) return;

            var payment = db.payments.FirstOrDefault(p => p.payment_id == paymentId);
            if (payment == null)
            {
                MessageBoxBuilder.ShowNotFound("Payment not found.");
                return;
            }

            db.payments.DeleteOnSubmit(payment);
            CRUDHelper.SafeSubmit(db); 
            MessageBoxBuilder.ShowSuccess("Payment successfully deleted");
        }

    }
}
