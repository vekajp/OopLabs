using System.Collections.Generic;
using Isu.Services;
using Isu.TimeEntities;

namespace IsuExtra.StructureEntities
{
    public interface IFacultyManager
    {
        public void AddFaculty(Faculty faculty);
        public void DeleteFaculty(Faculty faculty);

        public Faculty GetFacultyByName(string name);
        public Faculty FindFacultyByGroup(Group group);

        public Faculty FindFacultyByStudent(Student student);

        public void AddGroup(Group group);
        public void AddStudent(Student student);

        public Group FindGroupByName(string name);

        public IEnumerable<Student> GetStudentsByGroup(Group group);
    }
}