using System.Collections.Generic;
using System.Text.RegularExpressions;
using Isu.Tools;

namespace Isu.Services
{
    public class Group
    {
        private const int MaxStudentPerGroup = 20;

        public Group(string name)
        {
            if (Regex.IsMatch(name, @"M3[0-9]{3}"))
            {
                Name = name;
                CourseNumber = new CourseNumber(int.Parse(name[2].ToString()));
                Students = new List<Student>();
            }
            else
            {
                throw new IsuException("Invalid group format");
            }
        }

        public string Name { get; }

        public CourseNumber CourseNumber { get; }

        public List<Student> Students { get; }

        public Student AddStudent(string name)
        {
            var student = new Student(name, this);
            return AddStudent(student);
        }

        public Student AddStudent(Student student)
        {
            if (Students.Count >= MaxStudentPerGroup)
            {
                throw new IsuException("Too many students in the group");
            }

            Students.Add(student);
            return student;
        }

        public bool DeleteStudent(Student student)
        {
            return Students.Remove(student);
        }
    }
}