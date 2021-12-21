using System;
using NUnit.Framework;
using Reports.DAL.Entities.Employees;
using Reports.DAL.Entities.TaskRelatedEntities;
using Reports.Server.Services;

namespace Reports.Tests
{
    public class TaskServiceTests
    {
        private ITaskManager _manager;
        private IEmployeeService _employeeService;
        private Employee _testEmployee;
        private TeamLead _lead;
        private Employee[] _testEmployees;
        [SetUp]
        public void SetUp()
        {
            _manager = new TaskManager();
            _lead = new TeamLead("teamlead");
            _employeeService = new EmployeeService(_lead);
            _testEmployee = new Supervisor("test_employee");
            _testEmployees = new Employee[]
            {
                new Worker("worker1"),
                new Worker("worker2"),
                new Worker("worker3"),
            };
            _employeeService.AddEmployee(_testEmployee);
            foreach (Employee testEmployee in _testEmployees)
            {
                _employeeService.AddEmployee(testEmployee, _testEmployee);
            }
        }

        [Test]
        public void TestAddAndRemoveTask()
        {
            var task = new Task("task1", _testEmployee);
            _manager.AddTask(task);
            Assert.AreEqual(task, _manager.FindTaskById(task.Id));
            _manager.RemoveTask(task);
            Assert.AreEqual(null, _manager.FindTaskById(task.Id));
        }

        [Test]
        public void TestGetByDateLastChangeAndCreation()
        {
            DateTime creationTime = DateTime.Now;
            var task1 = new Task("task1", _testEmployee);
            var task2 = new Task("task2", _testEmployee);
            var task3 = new Task("task3", _testEmployee);
            _manager.AddTask(task1);
            _manager.AddTask(task2);
            _manager.AddTask(task3);
            DateTime changeTime = DateTime.Now;
            task1.LeaveComment(_testEmployee, "ыъ");
            task2.LeaveComment(_testEmployee, "ыъ");
            task3.LeaveComment(_testEmployee, "ыъ");

            // TODO uncomment
            // Assert.AreEqual(3, _manager.GetTasksByDateCreated(creationTime).Count);
            // Assert.AreEqual(3, _manager.GetTasksByDateLastModified(changeTime).Count);
        }

        [Test]
        public void TestGetTasksByEmployeeAndSubordinates()
        {
            var tasks = new Task[]
            {
                new Task("task1", _testEmployee),
                new Task("task2", _testEmployee),
                new Task("task3", _testEmployee),
                new Task("task4", _testEmployees[0]),
                new Task("task5", _testEmployees[1]),
                new Task("task6", _testEmployees[2]),
            };
            foreach (Task task in tasks)
            {
                _manager.AddTask(task);
            }

            Assert.AreEqual(_manager.GetTasksByEmployee(_testEmployee).Count, 3);
            Assert.AreEqual(_manager.GetTasksByEmployeeSubordinates(_testEmployee).Count, 3);
            Assert.AreEqual(_manager.GetTasksByEmployeeSubordinates(_lead).Count, 3);
        }

        [Test]
        public void GetTasksChangedBy()
        {
            var tasks = new Task[]
            {
                new Task("task1", _testEmployee),
                new Task("task4", _testEmployees[0]),
                new Task("task5", _testEmployees[1]),
                new Task("task6", _testEmployees[2]),
            };
            foreach (Task task in tasks)
            {
                _manager.AddTask(task);
            }

            tasks[1].LeaveComment(_testEmployee, "y");
            tasks[2].LeaveComment(_testEmployee, "y");

            tasks[0].LeaveComment(_testEmployees[0], "y");
            Assert.AreEqual(_manager.GetTasksModifiedByEmployee(_testEmployee).Count, 3);
            Assert.AreEqual(_manager.GetTasksModifiedByEmployee(_testEmployees[0]).Count, 2);
            Assert.AreEqual(_manager.GetTasksModifiedByEmployee(_testEmployees[1]).Count, 1);
            Assert.AreEqual(_manager.GetTasksModifiedByEmployee(_testEmployees[2]).Count, 1);
        }
    }
}