using System.Collections.Generic;
using Isu.Services;

namespace IsuExtra.StructureEntities
{
    public interface IElectiveManager
    {
        public void AddElective(Elective elective);
        public void RegisterStudent(Student student, Elective elective, string group);
        public void DeregisterStudent(Student student, Elective elective, string group);
        public IEnumerable<Student> GetStudentsFromDivision(Division division);
        public IEnumerable<Student> GetStudentsFromDivision(Elective elective, string group);
        public IEnumerable<Student> GetStudents(Elective elective);

        public Elective GetElectiveById(string id);

        public IEnumerable<Division> GetDivisions(Elective elective);
    }
}