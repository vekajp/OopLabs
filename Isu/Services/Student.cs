namespace Isu.Services
{
    public class Student
    {
        private static int _counter;
        public Student(string name, Group group)
        {
            Name = name;
            Id = _counter++;
            Group = group;
        }

        public Group Group { get; private set; }

        public int Id { get; }

        public string Name { get; }

        public void ChangeGroup(Group group)
        {
            Group = group;
        }
    }
}