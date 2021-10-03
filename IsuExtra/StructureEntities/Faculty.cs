using System;
using System.Collections.Generic;
using Isu.Services;

namespace IsuExtra.StructureEntities
{
    public class Faculty
    {
        private List<Group> _groups;
        private char _sign;
        public Faculty(string name, char sign)
        {
            Name = name;
            _groups = new List<Group>();
            _sign = sign;
        }

        public string Name { get; }
        public IReadOnlyList<Group> Groups => _groups;
        public override bool Equals(object obj)
        {
            if (obj != null && (obj.GetType() != this.GetType())) return false;
            return this.Equals((Faculty)obj);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public bool ContainsGroup(Group group)
        {
            return _groups.Contains(group);
        }

        public bool GroupBelongsToTheFaculty(Group group)
        {
            return group.GetSign() == _sign;
        }

        public void AddGroup(Group group)
        {
            if (ContainsGroup(group))
            {
                throw new ArgumentException("Group was already added", nameof(group));
            }

            _groups.Add(group);
        }

        public Group FindGroupByName(string name)
        {
            return _groups.Find(x => x.Name == name);
        }

        private bool Equals(Faculty other)
        {
            return Name == other.Name;
        }
    }
}