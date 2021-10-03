using System;
using System.Collections.Generic;
using System.Linq;
using Isu.Services;

namespace IsuExtra.StructureEntities
{
    public class FacultyManager : IFacultyManager
    {
        private readonly List<Faculty> _faculties;
        public FacultyManager()
        {
            _faculties = new List<Faculty>();
        }

        public IReadOnlyList<Student> Students
        {
            get
            {
                var students = new List<Student>();
                foreach (Group @group in _faculties.SelectMany(faculty => faculty.Groups))
                {
                    students.AddRange(@group.Students);
                }

                return students;
            }
        }

        public void AddFaculty(Faculty faculty)
        {
            if (_faculties.Contains(faculty)) throw new ArgumentException("Faculty already exists", nameof(faculty));

            _faculties.Add(faculty);
        }

        public void DeleteFaculty(Faculty faculty)
        {
            if (!_faculties.Contains(faculty)) throw new ArgumentException("Faculty already exists", nameof(faculty));

            _faculties.Remove(faculty);
        }

        public Faculty FindFacultyByGroup(Group group)
        {
            return _faculties.Find(x => x.GroupBelongsToTheFaculty(group));
        }

        public Faculty FindFacultyByStudent(Student student)
        {
            return _faculties.Find(x => x.ContainsGroup(student.Group));
        }

        public void AddGroup(Group group)
        {
            Faculty faculty = FindFacultyByGroup(group);
            if (faculty == null) throw new ArgumentException("Group doesn't belong to any of faculties", nameof(@group));

            faculty.AddGroup(group);
        }

        public void AddStudent(Student student)
        {
            student.Group.AddStudent(student);
        }

        public Group FindGroupByName(string name)
        {
            Group group = new Group(name);
            return _faculties.Find(x => x.GroupBelongsToTheFaculty(group))?.FindGroupByName(name);
        }

        public IEnumerable<Student> GetStudentsByGroup(Group @group)
        {
            return group.Students;
        }

        public Faculty GetFacultyByName(string name)
        {
            Faculty faculty = _faculties.Find(x => x.Name == name);
            if (faculty == null) throw new ArgumentException("Faculty not found", nameof(name));

            return faculty;
        }

        public Faculty GetFaculty(Group group)
        {
            Faculty faculty = _faculties.Find(x => x.GroupBelongsToTheFaculty(group));
            if (faculty == null) throw new ArgumentException("Faculty not found", nameof(@group));

            return faculty;
        }
    }
}