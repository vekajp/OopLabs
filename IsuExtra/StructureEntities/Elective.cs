using System;
using System.Collections.Generic;
using System.Linq;

namespace IsuExtra.StructureEntities
{
    public class Elective
    {
        private List<ElectiveGroup> _divisions;
        public Elective(string id, Faculty faculty, List<ElectiveGroup> divisions)
        {
            Id = id;
            Faculty = faculty;
            _divisions = divisions;
        }

        public IReadOnlyCollection<ElectiveGroup> Divisions => _divisions;

        public IReadOnlyCollection<IsuStudent> Students
        {
            get
            {
                return _divisions.SelectMany(division => division.Students).ToList();
            }
        }

        public Faculty Faculty { get; }
        public string Id { get; }

        public void AddStudent(IsuStudent student, string id)
        {
            ElectiveGroup electiveGroup = FindDivisionById(id);
            if (electiveGroup == null)
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

            electiveGroup.AddStudent(student);
        }

        public void RemoveStudent(IsuStudent student, string id)
        {
            ElectiveGroup electiveGroup = FindDivisionById(id);
            if (electiveGroup == null)
            {
                throw new ArgumentException("Division not found", nameof(id));
            }

            electiveGroup.RemoveStudent(student);
        }

        public IReadOnlyCollection<IsuStudent> GetStudents(string id)
        {
            ElectiveGroup electiveGroup = FindDivisionById(id);
            if (electiveGroup == null)
            {
                throw new ArgumentException("Division not found", nameof(id));
            }

            return electiveGroup.Students;
        }

        private ElectiveGroup FindDivisionById(string id)
        {
            return _divisions.Find(x => x.Id == id);
        }

        private ElectiveGroup FindDivisionByStudent(IsuStudent student)
        {
            return _divisions.Find(x => x.ContainsStudent(student));
        }

        private bool StudentIsRegistered(IsuStudent student)
        {
            return FindDivisionByStudent(student) != null;
        }
    }
}