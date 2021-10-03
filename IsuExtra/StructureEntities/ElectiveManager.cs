using System;
using System.Collections.Generic;
using System.Linq;

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

        public void RegisterStudent(IsuStudent student, Elective elective, string @group)
        {
            if (StudentCheckedIn(student))
            {
                throw new ArgumentException($"Student already chose {MaxElectives} electives", nameof(student));
            }

            elective.AddStudent(student, group);
        }

        public void DeregisterStudent(IsuStudent student, Elective elective, string @group)
        {
           elective.RemoveStudent(student, group);
        }

        public IEnumerable<IsuStudent> GetStudentsFromDivision(ElectiveGroup electiveGroup)
        {
            return electiveGroup.Students;
        }

        public IEnumerable<IsuStudent> GetStudentsFromDivision(Elective elective, string @group)
        {
            return elective.GetStudents(group);
        }

        public IEnumerable<IsuStudent> GetStudents(Elective elective)
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

        public IEnumerable<ElectiveGroup> GetDivisions(Elective elective)
        {
            return elective.Divisions;
        }

        public bool StudentCheckedIn(IsuStudent isuStudent)
        {
            return CountElectives(isuStudent) == MinElectives;
        }

        private int CountElectives(IsuStudent isuStudent)
        {
            return _electives.Count(x => x.Students.Contains(isuStudent));
        }
    }
}