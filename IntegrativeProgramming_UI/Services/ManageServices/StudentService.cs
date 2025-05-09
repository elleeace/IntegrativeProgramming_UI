using IntegrativeProgramming_UI.Models;
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
    internal class StudentService
    {
        private readonly NorthvilleLibraryDataContext db = new NorthvilleLibraryDataContext(Properties.Settings.Default.NorthvilleLibraryV2ConnectionString);

        public StudentService(NorthvilleLibraryDataContext db)
        {
            this.db = db;
        }

        public void AddStudent(StackPanel sp, Action onSuccess)
        {
            FormBuilder.BuildAddStudentForm(sp,db,onSuccess);
        }
        public ObservableCollection<Student> LoadStudentTable()
        {
            return new ObservableCollection<Student>(
                db.students.Select(s => new Student
                {
                    StudentID = s.student_id,
                    StudentName = s.student_name,
                    CourseID = s.course.course_id,
                    ContactNumber = s.contact_number
                }).ToList());
        }
        public void DeleteStudent(string studentId)
        {
            var student = db.students.FirstOrDefault(s => s.student_id == studentId);
            if (student != null)
            {
                try
                {
                    db.students.DeleteOnSubmit(student);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
        }


    }
}
