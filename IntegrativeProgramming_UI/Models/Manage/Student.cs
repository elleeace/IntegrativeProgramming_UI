using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrativeProgramming_UI.Models
{
    internal class Student
    {
        public string StudentID { get; set; }
        public string StudentName { get; set; }
        public string ContactNumber { get; set; }
        public string CourseID { get; set; }

        public Student()
        {
            
        }
        public Student(string studentID, string studentName, string cNumber, string course)
        {
            StudentID = studentID;
            StudentName = studentName;
            ContactNumber = cNumber;
            CourseID = course;
        }
    }
}
