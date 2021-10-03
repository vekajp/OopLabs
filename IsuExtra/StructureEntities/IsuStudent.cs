using Isu.Services;

namespace IsuExtra.StructureEntities
{
    public class IsuStudent
    {
        private Student _student;
        public IsuStudent(string name, IsuGroup group)
        {
            _student = new Student(name, null);
            Group = group;
        }

        public IsuStudent(Student student, IsuGroup @group)
        {
            _student = student;
            Group = group;
        }

        public IsuGroup Group { get; }

        public static implicit operator Student(IsuStudent student) => student._student;

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;
            IsuStudent other = (IsuStudent)obj;
            return _student.Equals(other._student) && Group.Equals(other.Group);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}