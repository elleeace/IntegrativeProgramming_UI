using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrativeProgramming_UI.Models
{
    internal class Course
    {
        public string CourseID { get; set; }
        public string CourseName { get; set; }

        public Course()
        {
            
        }
        public Course(string courseID, string courseName)
        {
            CourseID = courseID;
            CourseName = courseName;
        }
    }
}
