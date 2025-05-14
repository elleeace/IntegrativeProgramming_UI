using IntegrativeProgramming_UI.Helpers;
using IntegrativeProgramming_UI.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
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
            FormBuilder.BuildAddStudentForm(sp, db, onSuccess);
        }

        public void EditStudent(StackPanel formPanel, NorthvilleLibraryDataContext db, Student student, Action onSaved)
        {
            FormBuilder.BuildEditStudentForm(formPanel, db, student.StudentID, onSaved);
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
            if (string.IsNullOrWhiteSpace(studentId))
            {
                MessageBoxBuilder.ShowError("Could not find student ID " + studentId);
                return;
            }

            var student = db.students.FirstOrDefault(s => s.student_id == studentId);
            if (student != null)
            {
                var confirm = MessageBoxBuilder.ShowConfirm("Are you sure you want to delete Student ID: " + studentId + "?");

                if (confirm == MessageBoxResult.Yes)
                {
                    try
                    {
                        db.students.DeleteOnSubmit(student);
                        CRUDHelper.SafeSubmit(db);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }
                }
                else
                {
                    return;
                }

            }
            else
            {
                MessageBoxBuilder.ShowNotFound("Could not find student ID " + studentId);
            }
        }


    }
}
