using System.Collections.Generic;
using System.Linq;
using Isu.Services;
using IsuExtra.TimeEntities;

namespace IsuExtra.StructureEntities
{
    public class IsuGroup
    {
        public IsuGroup(string name, Schedule schedule)
        {
            Group = new Group(name);
            Schedule = schedule;
        }

        public IReadOnlyCollection<IsuStudent> Students
        {
            get
            {
               return Group.Students.Select(student => new IsuStudent(student, this)).ToList();
            }
        }

        public Schedule Schedule { get; }

        public string Name => Group.Name;
        private Group Group { get; }
        public static implicit operator Group(IsuGroup group) => group.Group;
        public virtual void AddStudent(IsuStudent isuStudent)
        {
            Group.AddStudent(isuStudent);
        }

        public void DeleteStudent(IsuStudent isuStudent)
        {
            Group.DeleteStudent(isuStudent);
        }

        public char GetSign()
        {
            return Group.Name[0];
        }
    }
}