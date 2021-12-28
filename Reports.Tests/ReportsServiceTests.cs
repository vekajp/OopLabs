using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Reports.DAL.Entities;
using Reports.DAL.Entities.Employees;
using Reports.DAL.Entities.TaskRelatedEntities;
using Reports.DAL.Tools;
using Reports.Server.DataBase;
using Reports.Server.Services;

namespace Reports.Tests
{
    public class ReportsServiceTests
    {
        private IReportService _service;
        private TeamLead _teamLead;
        private List<Employee> _employees;
        [SetUp]
        public void SetUp()
        {
            _service = new ReportService(CreateEmployeeService(), CreateTaskManager());
        }

        [Test]
        public void TestGetReport()
        {
            _service.Authorize(_teamLead);
            Assert.AreEqual(_service.GetEmployeesNotReported().Count, 3);
            _service.Authorize(_employees[6]);
            _service.ComposeWeeklyReport();
            _service.SendFinalReport();

            _service.Authorize(_teamLead);
            Assert.AreEqual(_service.GetEmployeesNotReported().Count, 2);
        }

        [Test]
        public void TestSendFinalReport()
        {
            _service.Authorize(_teamLead);
            Assert.Throws<ReportException>(() =>
            {
                _service.SendFinalReport();
            });

            _service.Authorize(_employees[0]);
            Assert.Throws<ReportException>(() =>
            {
                _service.SendFinalReport();
            });
            _service.Authorize(_employees[1]);
            Assert.Throws<ReportException>(() =>
            {
                _service.SendFinalReport();
            });
            _service.Authorize(_employees[2]);
            Assert.Throws<ReportException>(() =>
            {
                _service.SendFinalReport();
            });
            _service.Authorize(_employees[3]);
            _service.ComposeWeeklyReport();
            _service.SendFinalReport();
            _service.Authorize(_employees[4]);
            _service.ComposeWeeklyReport();
            _service.SendFinalReport();
            _service.Authorize(_employees[5]);
            _service.ComposeWeeklyReport();
            _service.SendFinalReport();
            _service.Authorize(_employees[6]);
            _service.ComposeWeeklyReport();
            _service.SendFinalReport();

            _service.Authorize(_employees[2]);
            _service.ComposeWeeklyReport();
            _service.SendFinalReport();
            _service.Authorize(_employees[1]);
            _service.ComposeWeeklyReport();
            _service.SendFinalReport();
            _service.Authorize(_employees[0]);
            _service.ComposeWeeklyReport();
            _service.SendFinalReport();
            _service.Authorize(_teamLead);
            _service.ComposeWeeklyReport();
            Report final = _service.SendFinalReport();
            Assert.AreEqual(final.Reports.Count, 3);
            Console.WriteLine(final.ToString());
        }

        [Test]
        public void TestEmployeesNotReportedChaeck()
        {
            _service.Authorize(_teamLead);
            Assert.AreEqual(_service.GetEmployeesNotReported().Count, 3);

            _service.Authorize(_employees[0]);
            Assert.AreEqual(_service.GetEmployeesNotReported().Count, 2);

            _service.Authorize(_employees[1]);
            Assert.AreEqual(_service.GetEmployeesNotReported().Count, 1);

            _service.Authorize(_employees[2]);
            Assert.AreEqual(_service.GetEmployeesNotReported().Count, 1);

            _service.Authorize(_employees[5]);
            _service.ComposeWeeklyReport();
            _service.SendFinalReport();

            _service.Authorize(_employees[3]);
            _service.ComposeWeeklyReport();
            _service.SendFinalReport();

            _service.Authorize(_employees[2]);
            Assert.AreEqual(_service.GetEmployeesNotReported().Count, 0);
            _service.ComposeWeeklyReport();
            _service.SendFinalReport();

            _service.Authorize(_employees[0]);
            Assert.AreEqual(_service.GetEmployeesNotReported().Count, 0);
            _service.ComposeWeeklyReport();
            _service.SendFinalReport();

            _service.Authorize(_teamLead);
            Assert.AreEqual(_service.GetEmployeesNotReported().Count, 2);
        }


        private IEmployeeService CreateEmployeeService()
        {
            _teamLead = new TeamLead("teamlead");
            Employee supervisor1 = new Supervisor("supervisor1");
            Employee supervisor2 = new Supervisor("supervisor2");
            Employee supervisor3 = new Supervisor("supervisor3");
            _employees = new List<Employee>
            {
                supervisor1,
                supervisor2,
                supervisor3,
                new Worker("smart_worker"),
                new Worker("not_so_smart_worker"),
                new Worker("not_smart_worker"),
                new Worker("simple_worker"),
            };
            var employeeService = new EmployeeService(_teamLead);

            Assert.That(employeeService.TryAddEmployee(supervisor1, _teamLead));
            Assert.That(employeeService.TryAddEmployee(supervisor2, _teamLead));
            Assert.That(employeeService.TryAddEmployee(supervisor3, supervisor1));
            Assert.That(employeeService.TryAddEmployee(_employees[3], supervisor1));
            Assert.That(employeeService.TryAddEmployee(_employees[4], supervisor2));
            Assert.That(employeeService.TryAddEmployee(_employees[5], supervisor3));
            Assert.That(employeeService.TryAddEmployee(_employees[6], _teamLead));
            return employeeService;
        }

        private ITaskManager CreateTaskManager()
        {
            var manager = new TaskManager();
            var task1 = new Task("task1", _employees[0]);
            var task2 = new Task("task2", _employees[1]);
            var task3 = new Task("task3", _employees[2]);
            manager.AddTask(task1);
            manager.AddTask(task2);
            manager.AddTask(task3);
            task1.AssignToEmployee(_teamLead, _employees[3]);
            task2.ChangeState(_employees[1], TaskState.Resolved);
            return manager;
        }
    }
}