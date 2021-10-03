using System;
using System.Collections.Generic;
using System.Linq;
using Isu.Services;

namespace IsuExtra.StructureEntities
{
    public class ElectiveManager : IElectiveManager
    {
        private const int MaxElectives = 2;
        private const int MinElectives = 2;
        private readonly List<Elective> _electives;
        public ElectiveManager()
        {
            _electives = new List<Elective>();
        }

        public void AddElective(Elective elective)
        {
            if (_electives.Contains(elective))
            {
                throw new ArgumentException("Elective already exists", nameof(elective));
            }

            _electives.Add(elective);
        }

        public void RegisterStudent(Student student, Elective elective, string @group)
        {
            if (StudentCheckedIn(student))
            {
                throw new ArgumentException($"Student already chose {MaxElectives} electives", nameof(student));
            }

            elective.AddStudent(student, group);
        }

        public void DeregisterStudent(Student student, Elective elective, string @group)
        {
           elective.RemoveStudent(student, group);
        }

        public IEnumerable<Student> GetStudentsFromDivision(Division division)
        {
            return division.Students;
        }

        public IEnumerable<Student> GetStudentsFromDivision(Elective elective, string @group)
        {
            return elective.GetStudents(group);
        }

        public IEnumerable<Student> GetStudents(Elective elective)
        {
            return elective.Students;
        }

        public Elective GetElectiveById(string id)
        {
            Elective elective = _electives.Find(x => x.Id == id);
            if (elective == null)
            {
                throw new ArgumentException("Elective not found", nameof(id));
            }

            return elective;
        }

        public IEnumerable<Division> GetDivisions(Elective elective)
        {
            return elective.Divisions;
        }

        public bool StudentCheckedIn(Student student)
        {
            return CountElectives(student) == MinElectives;
        }

        private int CountElectives(Student student)
        {
            return _electives.Count(x => x.Students.Contains(student));
        }
    }
}