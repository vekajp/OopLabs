using System.Collections.Generic;

namespace IsuExtra.StructureEntities
{
    public interface IElectiveManager
    {
        public void AddElective(Elective elective);
        public void RegisterStudent(IsuStudent student, Elective elective, string group);
        public void DeregisterStudent(IsuStudent student, Elective elective, string group);
        public IEnumerable<IsuStudent> GetStudentsFromDivision(ElectiveGroup electiveGroup);
        public IEnumerable<IsuStudent> GetStudentsFromDivision(Elective elective, string group);
        public IEnumerable<IsuStudent> GetStudents(Elective elective);

        public Elective GetElectiveById(string id);

        public IEnumerable<ElectiveGroup> GetDivisions(Elective elective);
    }
}