using IntegrativeProgramming_UI.Helpers;
using IntegrativeProgramming_UI.Models;
using IntegrativeProgramming_UI.Models.Catalog;
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
    internal class AttendanceService
    {
        private readonly NorthvilleLibraryDataContext db = new NorthvilleLibraryDataContext(Properties.Settings.Default.NorthvilleLibraryV2ConnectionString);

        public AttendanceService(NorthvilleLibraryDataContext _db)
        {
            this.db = _db;
        }

        public void CreateAttendance(StackPanel sp, Action onSuccess)
        {
            FormBuilder.BuildAttendanceForm(sp, db, onSuccess);
        }

        public ObservableCollection<AttendanceDisplay> LoadAttendanceTable()
        {
            return new ObservableCollection<AttendanceDisplay>(
                from at in db.attendances
                select new AttendanceDisplay
                {
                    AttendanceID = at.attendance_id,
                    StudentID = at.student_id,
                    StudentName = at.student.student_name,
                    Course = at.student.course.course_name,
                    VisitDate = at.date_of_visit,
                    Time = at.time_of_visit
                });
        }


        public void EditAttendance(StackPanel formPanel, NorthvilleLibraryDataContext db, AttendanceDisplay attendance, Action onSaved)
        {
            FormBuilder.BuildEditAttendanceForm(formPanel, db, attendance.AttendanceID, onSaved);
        }


        public void DeleteAttendance(string attendanceId)
        {
            var attendance = db.attendances.FirstOrDefault(a => a.attendance_id == attendanceId);
            if (attendance != null)
            {
                db.attendances.DeleteOnSubmit(attendance);
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
