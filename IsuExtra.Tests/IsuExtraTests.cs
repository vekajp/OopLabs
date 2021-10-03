using System;
using System.Linq;
using Isu.Services;
using Isu.TimeEntities;
using IsuExtra.StructureEntities;
using NUnit.Framework;
using TimeSpan = Isu.TimeEntities.TimeSpan;

namespace IsuExtra.Tests
{
    public class IsuExtraTests
    {
        private IIsuExtraService _isuExtra;

        [SetUp]
        public void SetUp()
        {
            //register faculties
            _isuExtra = new IsuExtraService();
            _isuExtra.AddFaculty(new Faculty("FITIP", 'M'));
            _isuExtra.AddFaculty(new Faculty("FTF", 'P'));
            _isuExtra.AddFaculty(new Faculty("SMTH", 'N'));

            //Create schedules
            ScheduleBuilder scheduleBuilder = new ScheduleBuilder();
            scheduleBuilder.AddLesson(DayOfWeek.Monday, 
                new TimeSpan(AcademicClass.One),
                "teacher1",
                1);
            Schedule schedule1 = scheduleBuilder.MakeSchedule();
            scheduleBuilder.AddLesson(DayOfWeek.Tuesday,
                new TimeSpan(AcademicClass.One),
                "teacher2",
                2);
            Schedule schedule2 = scheduleBuilder.MakeSchedule();
            scheduleBuilder.AddLesson(DayOfWeek.Wednesday,
                new TimeSpan(AcademicClass.One),
                "teacher3",
                3);
            Schedule schedule3 = scheduleBuilder.MakeSchedule();
            
            //Create groups
            Group gr1 = new Group("M3200", schedule1);
            Group gr2 = new Group("P3202", schedule2);
            Group gr3 = new Group("N3204", schedule3);
            
            _isuExtra.AddGroup(gr1);
            _isuExtra.AddGroup(gr2);
            _isuExtra.AddGroup(gr3);

            //Add electives
            Faculty fifip = _isuExtra.GetFacultyByName("FITIP");
            Faculty ftf = _isuExtra.GetFacultyByName("FTF");
            Faculty smth = _isuExtra.GetFacultyByName("SMTH");
            ElectiveBuilder electiveBuilder = new ElectiveBuilder("ognp_fitip", fifip);
            
            scheduleBuilder = new ScheduleBuilder();
            scheduleBuilder.AddLesson(DayOfWeek.Monday, 
                new TimeSpan(AcademicClass.Two),
                "teacher1",
                1);
            electiveBuilder.AddDivision(new Division("1", scheduleBuilder.MakeSchedule()));
            _isuExtra.AddElective(electiveBuilder.MakeElective());
            
            scheduleBuilder.Destroy();
            electiveBuilder = new ElectiveBuilder("ognp_ftf", ftf);
            scheduleBuilder.AddLesson(DayOfWeek.Tuesday,
                new TimeSpan(AcademicClass.Two),
                "teacher2",
                2);
            electiveBuilder.AddDivision(new Division("1", scheduleBuilder.MakeSchedule()));
            electiveBuilder.AddDivision(new Division("2", scheduleBuilder.MakeSchedule()));
            _isuExtra.AddElective(electiveBuilder.MakeElective());
            
            scheduleBuilder.Destroy();
            electiveBuilder = new ElectiveBuilder("ognp_smth", smth);
            scheduleBuilder.AddLesson(DayOfWeek.Wednesday,
                new TimeSpan(AcademicClass.Two),
                "teacher3",
                3);
            electiveBuilder.AddDivision(new Division("1", scheduleBuilder.MakeSchedule()));
            _isuExtra.AddElective(electiveBuilder.MakeElective());
            
            scheduleBuilder.Destroy();
            electiveBuilder = new ElectiveBuilder("ognp_extra", smth);
            scheduleBuilder.AddLesson(DayOfWeek.Thursday,
                new TimeSpan(AcademicClass.Six),
                "teacher3",
                3);
            electiveBuilder.AddDivision(new Division("1", scheduleBuilder.MakeSchedule()));
            _isuExtra.AddElective(electiveBuilder.MakeElective());
        }
        
        [Test]
        public void AddElective()
        {
            Faculty ftf = _isuExtra.GetFacultyByName("FTF");
            ElectiveBuilder electiveBuilder = new ElectiveBuilder("Photonics", ftf);
            
            electiveBuilder.AddDivision(new Division("group1", null));
            electiveBuilder.AddDivision(new Division("group2", null));
            _isuExtra.AddElective(electiveBuilder.MakeElective());
            
            Elective elective = _isuExtra.GetElectiveById("Photonics");
            Assert.That(elective != null);

            Assert.Catch<Exception>(() =>
            {
                elective = _isuExtra.GetElectiveById("some_elective");
            });

            Assert.Catch<Exception>(() =>
            {
                _isuExtra.AddElective(elective);
            });
        }

        [Test]
        public void TestCheckInAndCheckOut()
        {
            Student student = new Student("student_test", _isuExtra.FindGroupByName("M3200"));
            _isuExtra.AddStudent(student);
            Elective ftfOgnp = _isuExtra.GetElectiveById("ognp_ftf");
            Elective smthOgnp = _isuExtra.GetElectiveById("ognp_smth");
            
            Assert.That(_isuExtra.StudentsNotCheckedIn().Contains(student));
            _isuExtra.RegisterStudent(student, ftfOgnp, "1");
            Assert.That(_isuExtra.StudentsNotCheckedIn().Contains(student));
            _isuExtra.RegisterStudent(student, smthOgnp, "1");
            Assert.That(!_isuExtra.StudentsNotCheckedIn().Contains(student));

            Assert.That(_isuExtra.GetStudents(ftfOgnp).Contains(student));
            Assert.That(_isuExtra.GetStudents(smthOgnp).Contains(student));
            
            _isuExtra.DeregisterStudent(student, smthOgnp, "1");
            Assert.That(_isuExtra.StudentsNotCheckedIn().Contains(student));
            Assert.That(!smthOgnp.Students.Contains(student));
            
            Assert.Catch<Exception>(() =>
            {
                _isuExtra.RegisterStudent(student, ftfOgnp, "2");
            });
        }

        [Test]
        public void TestCheckInStudentOnHisFaculty()
        {
            Student student = new Student("test", _isuExtra.FindGroupByName("M3200"));
            _isuExtra.AddStudent(student);
            Elective fitipOgnp = _isuExtra.GetElectiveById("ognp_fitip");
            Assert.Catch<Exception>(() =>
            {
                _isuExtra.RegisterStudent(student, fitipOgnp, "1");
            });
            
        }
        
        [Test]
        public void TestCheckInStudentOnExtraElective()
        {
            Student student = new Student("student_test", _isuExtra.FindGroupByName("M3200"));
            _isuExtra.AddStudent(student);
            Elective ftfOgnp = _isuExtra.GetElectiveById("ognp_ftf");
            Elective smthOgnp = _isuExtra.GetElectiveById("ognp_smth");
            Elective extraOgnp = _isuExtra.GetElectiveById("ognp_extra");
            
            _isuExtra.RegisterStudent(student, ftfOgnp, "1");
            _isuExtra.RegisterStudent(student, smthOgnp, "1");
            Assert.Catch<Exception>(() =>
            {
                _isuExtra.RegisterStudent(student, extraOgnp, "1");
            });
        }
        
        [Test]
        public void TestCheckInStudentOnScheduleIntersects()
        {
            Student student = new Student("student_test", _isuExtra.FindGroupByName("M3200"));
            _isuExtra.AddStudent(student);
            ElectiveBuilder electiveBuilder = new ElectiveBuilder("ognp_extra", _isuExtra.GetFacultyByName("SMTH"));
            ScheduleBuilder scheduleBuilder = new ScheduleBuilder();
            scheduleBuilder.AddLesson(DayOfWeek.Monday,
                new TimeSpan(AcademicClass.One),
                "teacher3",
                3);
            electiveBuilder.AddDivision(new Division("1", scheduleBuilder.MakeSchedule()));
            Elective elective = electiveBuilder.MakeElective();
            _isuExtra.AddElective(elective);
            Assert.Catch<Exception>(() =>
            {
                _isuExtra.RegisterStudent(student, elective, "1");
            });
        }
    }
}