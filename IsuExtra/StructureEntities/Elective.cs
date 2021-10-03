using System;
using System.Collections.Generic;
using System.Linq;
using Isu.Services;

namespace IsuExtra.StructureEntities
{
    public class Elective
    {
        private List<Division> _divisions;
        public Elective(string id, Faculty faculty, List<Division> divisions)
        {
            Id = id;
            Faculty = faculty;
            _divisions = divisions;
        }

        public IReadOnlyList<Division> Divisions => _divisions;

        public IEnumerable<Student> Students
        {
            get
            {
                IEnumerable<Student> students = new List<Student>();
                return _divisions.Aggregate(students, (current, division) => current.Concat(division.Students));
            }
        }

        public Faculty Faculty { get; }
        public string Id { get; }

        public void AddStudent(Student student, string id)
        {
            Division division = FindDivisionById(id);
            if (division == null)
            {
                throw new ArgumentException("Division not found", nameof(id));
            }

            if (Faculty.GroupBelongsToTheFaculty(student.Group))
            {
                throw new ArgumentException("Cannot register for that course", nameof(student));
            }

            if (StudentIsRegistered(student))
            {
                throw new ArgumentException("Cannot register student for the same elective twice", nameof(student));
            }

            division.AddStudent(student);
        }

        public void RemoveStudent(Student student, string id)
        {
            Division division = FindDivisionById(id);
            if (division == null)
            {
                throw new ArgumentException("Division not found", nameof(id));
            }

            division.RemoveStudent(student);
        }

        public IEnumerable<Student> GetStudents(string id)
        {
            Division division = FindDivisionById(id);
            if (division == null)
            {
                throw new ArgumentException("Division not found", nameof(id));
            }

            return division.Students;
        }

        private Division FindDivisionById(string id)
        {
            return _divisions.Find(x => x.Id == id);
        }

        private Division FindDivisionByStudent(Student student)
        {
            return _divisions.Find(x => x.ContainsStudent(student));
        }

        private bool StudentIsRegistered(Student student)
        {
            return FindDivisionByStudent(student) != null;
        }
    }
}