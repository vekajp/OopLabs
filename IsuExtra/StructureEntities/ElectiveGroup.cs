using System;
using System.Collections.Generic;
using IsuExtra.TimeEntities;

namespace IsuExtra.StructureEntities
{
    public class ElectiveGroup
    {
        private const int MaxStudents = 20;
        private List<IsuStudent> _students;

        public ElectiveGroup(string id, Schedule schedule)
        {
            Id = id;
            Schedule = schedule;
            _students = new List<IsuStudent>();
        }

        public Schedule Schedule { get; }
        public IReadOnlyList<IsuStudent> Students => _students;
        public string Id { get; }

        public void AddStudent(IsuStudent student)
        {
            if (_students.Count == MaxStudents)
                throw new ArgumentOutOfRangeException(nameof(_students), "Elective's group is full");
            if (student.Group.Schedule.Intersects(Schedule))
            {
                throw new ArgumentException("Student's schedule intersects with elective schedule", nameof(student));
            }

            _students.Add(student);
        }

        public void RemoveStudent(IsuStudent student)
        {
            if (!_students.Contains(student))
                throw new ArgumentException("Student not found", nameof(student));
            _students.Remove(student);
        }

        public bool ContainsStudent(IsuStudent student)
        {
            return _students.Contains(student);
        }
    }
}