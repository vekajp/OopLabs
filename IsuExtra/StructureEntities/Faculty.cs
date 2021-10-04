using System;
using System.Collections.Generic;

namespace IsuExtra.StructureEntities
{
    public class Faculty
    {
        private List<IsuGroup> _groups;
        private char _sign;
        public Faculty(string name, char sign)
        {
            Name = name;
            _groups = new List<IsuGroup>();
            _sign = sign;
        }

        public string Name { get; }
        public IReadOnlyCollection<IsuGroup> Groups => _groups;
        public override bool Equals(object obj)
        {
            if (obj != null && (obj.GetType() != this.GetType())) return false;
            return this.Equals((Faculty)obj);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public bool ContainsGroup(IsuGroup group)
        {
            return _groups.Contains(group);
        }

        public bool GroupBelongsToTheFaculty(IsuGroup group)
        {
            return group.GetSign() == _sign;
        }

        public void AddGroup(IsuGroup group)
        {
            if (ContainsGroup(group))
            {
                throw new ArgumentException("Group was already added", nameof(group));
            }

            _groups.Add(group);
        }

        public IsuGroup FindGroupByName(string name)
        {
            return _groups.Find(x => x.Name == name);
        }

        private bool Equals(Faculty other)
        {
            return Name == other.Name;
        }
    }
}