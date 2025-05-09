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
    internal class CourseService
    {
        private readonly NorthvilleLibraryDataContext db = new NorthvilleLibraryDataContext(Properties.Settings.Default.NorthvilleLibraryV2ConnectionString);

        public CourseService(NorthvilleLibraryDataContext db)
        {
            this.db = db;
        }

        public void CreateCourse(StackPanel sp, Action onSucess)
        {
            FormBuilder.BuildAddCourseForm(sp, db, onSucess);
        }

        public ObservableCollection<Course> LoadCourseTable()
        {
            return new ObservableCollection<Course>(
                (from ct in db.courses
                 select new Course
                 {
                     CourseID = ct.course_id,
                     CourseName = ct.course_name
                 }).ToList()
            );
        }


        public void DeleteCourse(string courseId)
        {
            var course = db.courses.FirstOrDefault(c => c.course_id == courseId);

            if (course != null)
            {
                // Remove students in this course first
                var studentsInCourse = db.students.Where(s => s.course_id == courseId).ToList();
                db.students.DeleteAllOnSubmit(studentsInCourse);
                db.courses.DeleteOnSubmit(course);

                try
                {
                    db.SubmitChanges();
                    MessageBox.Show("Record deleted successfully.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Delete failed: " + ex.Message);
                }
            }
        }

    }
}
