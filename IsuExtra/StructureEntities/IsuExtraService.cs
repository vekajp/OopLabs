using System.Collections.Generic;
using System.Linq;
using Isu.Services;

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

        public Faculty FindFacultyByGroup(Group @group)
        {
            return _facultyManager.FindFacultyByGroup(group);
        }

        public Faculty FindFacultyByStudent(Student student)
        {
            return _facultyManager.FindFacultyByStudent(student);
        }

        public void AddGroup(Group group)
        {
            _facultyManager.AddGroup(group);
        }

        public void AddStudent(Student student)
        {
            _facultyManager.AddStudent(student);
        }

        public Group FindGroupByName(string name)
        {
            return _facultyManager.FindGroupByName(name);
        }

        public IEnumerable<Student> GetStudentsByGroup(Group @group)
        {
            return group.Students;
        }

        public void AddElective(Elective elective)
        {
            _electiveManager.AddElective(elective);
        }

        public void RegisterStudent(Student student, Elective elective, string @group)
        {
            _electiveManager.RegisterStudent(student, elective, group);
        }

        public void DeregisterStudent(Student student, Elective elective, string @group)
        {
            _electiveManager.DeregisterStudent(student, elective, group);
        }

        public IEnumerable<Student> GetStudentsFromDivision(Division division)
        {
            return _electiveManager.GetStudentsFromDivision(division);
        }

        public IEnumerable<Student> GetStudentsFromDivision(Elective elective, string @group)
        {
            return _electiveManager.GetStudentsFromDivision(elective, group);
        }

        public IEnumerable<Student> GetStudents(Elective elective)
        {
            return _electiveManager.GetStudents(elective);
        }

        public Elective GetElectiveById(string id)
        {
            return _electiveManager.GetElectiveById(id);
        }

        public IEnumerable<Division> GetDivisions(Elective elective)
        {
            return elective.Divisions;
        }

        public IEnumerable<Student> StudentsNotCheckedIn()
        {
            return _facultyManager.Students.Where(x => !_electiveManager.StudentCheckedIn(x));
        }
    }
}