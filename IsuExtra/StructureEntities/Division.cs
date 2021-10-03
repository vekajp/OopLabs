using System;
using System.Collections.Generic;
using Isu.Services;
using Isu.TimeEntities;

namespace IsuExtra.StructureEntities
{
    public class Division
    {
        private const int MaxStudents = 20;
        private List<Student> _students;

        public Division(string id, Schedule schedule)
        {
            Id = id;
            Schedule = schedule;
            _students = new List<Student>();
        }

        public Schedule Schedule { get; }
        public IReadOnlyList<Student> Students => _students;
        public string Id { get; }

        public void AddStudent(Student student)
        {
            if (_students.Count == MaxStudents)
                throw new ArgumentOutOfRangeException(nameof(_students), "Elective's group is full");
            if (student.Group.Schedule.Intersects(Schedule))
            {
                throw new ArgumentException("Student's schedule intersects with elective schedule", nameof(student));
            }

            _students.Add(student);
        }

        public void RemoveStudent(Student student)
        {
            if (!_students.Contains(student))
                throw new ArgumentException("Student not found", nameof(student));
            _students.Remove(student);
        }

        public bool ContainsStudent(Student student)
        {
            return _students.Contains(student);
        }
    }
}