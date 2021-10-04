using System.Collections.Generic;

namespace IsuExtra.StructureEntities
{
    public interface IElectiveManager
    {
        void AddElective(Elective elective);
        void RegisterStudent(IsuStudent student, Elective elective, string group);
        void DeregisterStudent(IsuStudent student, Elective elective, string group);
        IReadOnlyCollection<IsuStudent> GetStudentsFromDivision(ElectiveGroup electiveGroup);
        IReadOnlyCollection<IsuStudent> GetStudentsFromDivision(Elective elective, string group);
        IReadOnlyCollection<IsuStudent> GetStudents(Elective elective);

        Elective GetElectiveById(string id);

        IReadOnlyCollection<ElectiveGroup> GetDivisions(Elective elective);
    }
}