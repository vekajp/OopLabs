using System.Collections.Generic;
using System.Linq;

namespace Isu.Services
{
   public class IsuService : IIsuService
    {
        public IsuService()
        {
            Students = new List<Student>();
            Groups = new List<Group>();
        }

        private List<Student> Students { get; }

        private List<Group> Groups { get; }

        public Group AddGroup(string name)
        {
            var group = new Group(name);
            Groups.Add(group);
            return group;
        }

        public Student AddStudent(Group @group, string name)
        {
            Student student = @group.AddStudent(name);
            if (student != null)
            {
                Students.Add(student);
            }

            return student;
        }

        public Student GetStudent(int id)
        {
            return Students.FirstOrDefault(student => student.Id == id);
        }

        public Student FindStudent(string name)
        {
            return Students.FirstOrDefault(student => student.Name == name);
        }

        public List<Student> FindStudents(string groupName)
        {
            var studentList = new List<Student>();
            foreach (Group @group in Groups.Where(@group => @group.Name == groupName))
            {
                studentList = @group.Students;
            }

            return studentList;
        }

        public List<Student> FindStudents(CourseNumber courseNumber)
        {
            return Groups.Where(@group => @group.CourseNumber == courseNumber).SelectMany(@group => @group.Students).ToList();
        }

        public Group FindGroup(string groupName)
        {
            return Groups.FirstOrDefault(@group => @group.Name == groupName);
        }

        public List<Group> FindGroups(CourseNumber courseNumber)
        {
            return Groups.Where(@group => @group.CourseNumber == courseNumber).ToList();
        }

        public void ChangeStudentGroup(Student student, Group newGroup)
        {
            Group oldGroup = student.Group;
            oldGroup.DeleteStudent(student);
            student.ChangeGroup(newGroup);
            newGroup.AddStudent(student);
        }
    }
}