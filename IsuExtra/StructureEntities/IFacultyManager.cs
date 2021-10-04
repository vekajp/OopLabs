using System.Collections.Generic;

namespace IsuExtra.StructureEntities
{
    public interface IFacultyManager
    {
        void AddFaculty(Faculty faculty);
        void DeleteFaculty(Faculty faculty);

        Faculty GetFacultyByName(string name);
        Faculty FindFacultyByGroup(IsuGroup group);

        Faculty FindFacultyByStudent(IsuStudent student);

        void AddGroup(IsuGroup group);
        void AddStudent(IsuStudent student);

        IsuGroup FindGroupByName(string name);

        IReadOnlyCollection<IsuStudent> GetStudentsByGroup(IsuGroup group);
    }
}