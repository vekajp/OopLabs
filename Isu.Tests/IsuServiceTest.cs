using Isu.Services;
using Isu.Tools;
using NUnit.Framework;

namespace Isu.Tests
{
    public class Tests
    {
        private IIsuService _isuService;

        [SetUp]
        public void Setup()
        {
            _isuService = new IsuService();
            string[] groupNames = {"M3200", "M3201", "M3202", "M3203", "M3204", "M3205"};
            foreach (string name in groupNames)
            {
                _isuService.AddGroup(name);
                for (int i = 0; i < 10; i++)
                {
                    _isuService.AddStudent(new Group(name), name + i.ToString());
                }
            }
            
        }

        [Test]
        public void AddStudentToGroup_StudentHasGroupAndGroupContainsStudent()
        {
            Group group = _isuService.FindGroup("M3200");
            Student student = _isuService.AddStudent(group, "dude1");
            Assert.Contains(student, _isuService.FindStudents("M3200"));
            Assert.AreEqual(group, student.Group);
        }

        [Test]
        public void ReachMaxStudentPerGroup_ThrowException()
        {
            var group = new Group("M3201");
            Assert.Catch<IsuException>(() =>
            {
                for (int i = 0; i < 21; i++)
                {
                    _isuService.AddStudent(group, i.ToString());
                }
            });
        }

        [Test]
        public void CreateGroupWithInvalidName_ThrowException()
        {
            Assert.Catch<IsuException>(() =>
            {
                _isuService.AddGroup("M%*766");
            });
        }

        [Test]
        public void TransferStudentToAnotherGroup_GroupChanged()
        {
            Group oldGroup = _isuService.FindGroup("M3203");
            Group newGroup = _isuService.FindGroup("M3204");
               
            _isuService.AddStudent(oldGroup, "Shreks");
            Student student = _isuService.FindStudent("Shreks");
            _isuService.ChangeStudentGroup(student, newGroup);
            
            Assert.AreEqual(newGroup, student.Group);
            Assert.Contains(student, _isuService.FindStudents("M3204"));
            
        }
    }
}