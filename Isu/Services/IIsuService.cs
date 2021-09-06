using System;
using System.Collections.Generic;
using System.Linq;

namespace Isu.Services
{
    public interface IIsuService
    {
        Group AddGroup(string name);
        Student AddStudent(Group group, string name);

        Student GetStudent(int id);
        Student FindStudent(string name);
        List<Student> FindStudents(string groupName);
        List<Student> FindStudents(CourseNumber courseNumber);

        Group FindGroup(string groupName);
        List<Group> FindGroups(CourseNumber courseNumber);

        void ChangeStudentGroup(Student student, Group newGroup);
    }

    public class IsuService : IIsuService
    {
        public IsuService()
        {
            Students = new List<Student>();
            Groups = new List<Group>();
        }

        private List<Student> Students
        {
            get;
        }

        private List<Group> Groups
        {
            get;
        }

        public Group AddGroup(string name)
        {
            var group = new Group(name);
            if (!Groups.Contains(group))
            {
                Groups.Add(group);
            }

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
            foreach (Student student in Students)
            {
                if (student.Id == id)
                {
                    return student;
                }
            }

            return null;
        }

        public Student FindStudent(string name)
        {
            foreach (Student student in Students)
            {
                if (student.Name == name)
                {
                    return student;
                }
            }

            return null;
        }

        public List<Student> FindStudents(string groupName)
        {
            var studentList = new List<Student>();
            foreach (Group group in Groups)
            {
                if (group.Name == groupName)
                {
                    studentList = group.Students;
                }
            }

            return studentList;
        }

        public List<Student> FindStudents(CourseNumber courseNumber)
        {
            var studentList = new List<Student>();
            foreach (Group group in Groups)
            {
                if (group.CourseNumber == courseNumber)
                {
                    foreach (Student student in group.Students)
                    {
                        studentList.Add(student);
                    }
                }
            }

            return studentList;
        }

        public Group FindGroup(string groupName)
        {
            foreach (Group group in Groups)
            {
                if (group.Name == groupName)
                {
                    return group;
                }
            }

            return null;
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