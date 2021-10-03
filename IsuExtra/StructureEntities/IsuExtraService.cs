using System.Collections.Generic;
using System.Linq;

namespace IsuExtra.StructureEntities
{
    public class IsuExtraService : IIsuExtraService
    {
        private readonly FacultyManager _facultyManager;
        private readonly ElectiveManager _electiveManager;

        public IsuExtraService()
        {
            _facultyManager = new FacultyManager();
            _electiveManager = new ElectiveManager();
        }

        public void AddFaculty(Faculty faculty)
        {
            _facultyManager.AddFaculty(faculty);
        }

        public void DeleteFaculty(Faculty faculty)
        {
            _facultyManager.DeleteFaculty(faculty);
        }

        public Faculty GetFacultyByName(string name)
        {
            return _facultyManager.GetFacultyByName(name);
        }

        public Faculty FindFacultyByGroup(IsuGroup @group)
        {
            return _facultyManager.FindFacultyByGroup(group);
        }

        public Faculty FindFacultyByStudent(IsuStudent student)
        {
            return _facultyManager.FindFacultyByStudent(student);
        }

        public void AddGroup(IsuGroup group)
        {
            _facultyManager.AddGroup(group);
        }

        public void AddStudent(IsuStudent student)
        {
            _facultyManager.AddStudent(student);
        }

        public IsuGroup FindGroupByName(string name)
        {
            return _facultyManager.FindGroupByName(name);
        }

        public IEnumerable<IsuStudent> GetStudentsByGroup(IsuGroup @group)
        {
            return group.Students;
        }

        public void AddElective(Elective elective)
        {
            _electiveManager.AddElective(elective);
        }

        public void RegisterStudent(IsuStudent student, Elective elective, string @group)
        {
            _electiveManager.RegisterStudent(student, elective, group);
        }

        public void DeregisterStudent(IsuStudent student, Elective elective, string @group)
        {
            _electiveManager.DeregisterStudent(student, elective, group);
        }

        public IEnumerable<IsuStudent> GetStudentsFromDivision(ElectiveGroup electiveGroup)
        {
            return _electiveManager.GetStudentsFromDivision(electiveGroup);
        }

        public IEnumerable<IsuStudent> GetStudentsFromDivision(Elective elective, string @group)
        {
            return _electiveManager.GetStudentsFromDivision(elective, group);
        }

        public IEnumerable<IsuStudent> GetStudents(Elective elective)
        {
            return _electiveManager.GetStudents(elective);
        }

        public Elective GetElectiveById(string id)
        {
            return _electiveManager.GetElectiveById(id);
        }

        public IEnumerable<ElectiveGroup> GetDivisions(Elective elective)
        {
            return elective.Divisions;
        }

        public IEnumerable<IsuStudent> StudentsNotCheckedIn()
        {
            return _facultyManager.Students.Where(x => !_electiveManager.StudentCheckedIn(x));
        }
    }
}