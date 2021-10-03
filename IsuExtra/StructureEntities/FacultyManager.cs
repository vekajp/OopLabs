using System;
using System.Collections.Generic;
using System.Linq;

namespace IsuExtra.StructureEntities
{
    public class FacultyManager : IFacultyManager
    {
        private readonly List<Faculty> _faculties;
        public FacultyManager()
        {
            _faculties = new List<Faculty>();
        }

        public IReadOnlyList<IsuStudent> Students
        {
            get
            {
                var students = new List<IsuStudent>();
                foreach (IsuGroup @group in _faculties.SelectMany(faculty => faculty.Groups))
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

        public Faculty FindFacultyByGroup(IsuGroup group)
        {
            return _faculties.Find(x => x.GroupBelongsToTheFaculty(group));
        }

        public Faculty FindFacultyByStudent(IsuStudent student)
        {
            return _faculties.Find(x => x.ContainsGroup(student.Group));
        }

        public void AddGroup(IsuGroup group)
        {
            Faculty faculty = FindFacultyByGroup(group);
            if (faculty == null) throw new ArgumentException("IsuGroup doesn't belong to any of faculties", nameof(@group));

            faculty.AddGroup(group);
        }

        public void AddStudent(IsuStudent student)
        {
            student.Group.AddStudent(student);
        }

        public IsuGroup FindGroupByName(string name)
        {
            IsuGroup group = new IsuGroup(name, null);
            return _faculties.Find(x => x.GroupBelongsToTheFaculty(group))?.FindGroupByName(name);
        }

        public IEnumerable<IsuStudent> GetStudentsByGroup(IsuGroup @group)
        {
            return group.Students;
        }

        public Faculty GetFacultyByName(string name)
        {
            Faculty faculty = _faculties.Find(x => x.Name == name);
            if (faculty == null) throw new ArgumentException("Faculty not found", nameof(name));

            return faculty;
        }

        public Faculty GetFaculty(IsuGroup group)
        {
            Faculty faculty = _faculties.Find(x => x.GroupBelongsToTheFaculty(group));
            if (faculty == null) throw new ArgumentException("Faculty not found", nameof(@group));

            return faculty;
        }
    }
}