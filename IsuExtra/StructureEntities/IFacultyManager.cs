using System.Collections.Generic;

namespace IsuExtra.StructureEntities
{
    public interface IFacultyManager
    {
        public void AddFaculty(Faculty faculty);
        public void DeleteFaculty(Faculty faculty);

        public Faculty GetFacultyByName(string name);
        public Faculty FindFacultyByGroup(IsuGroup group);

        public Faculty FindFacultyByStudent(IsuStudent student);

        public void AddGroup(IsuGroup group);
        public void AddStudent(IsuStudent student);

        public IsuGroup FindGroupByName(string name);

        public IEnumerable<IsuStudent> GetStudentsByGroup(IsuGroup group);
    }
}