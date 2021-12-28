using System.Collections.Generic;
using NUnit.Framework;
using Reports.DAL.Entities.Employees;
using Reports.Server.Services;

namespace Reports.Tests
{
    public class EmployeeServiceTests
    {
        private IEmployeeService _employeeService;
        private List<Employee> _employees;
        private TeamLead _teamLead;
        [SetUp]
        public void Setup()
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
            _employeeService = new EmployeeService(_teamLead);

            _employeeService.TryAddEmployee(supervisor1, _teamLead);
            _employeeService.TryAddEmployee(supervisor2, _teamLead);
            _employeeService.TryAddEmployee(supervisor3, supervisor1);
            _employeeService.TryAddEmployee(_employees[3], supervisor1);
            _employeeService.TryAddEmployee(_employees[4], supervisor2);
            _employeeService.TryAddEmployee(_employees[5], supervisor3);
            _employeeService.TryAddEmployee(_employees[6], _teamLead);
        }

        [Test]
        public void TestSupervisors()
        {
            Assert.AreEqual(_employees[0].GetSupervisor(), _teamLead);
            Assert.AreEqual(_employees[1].GetSupervisor(), _teamLead);
            Assert.AreEqual(_employees[2].GetSupervisor(), _employees[0]);
            Assert.AreEqual(_employees[3].GetSupervisor(), _employees[0]);
            Assert.AreEqual(_employees[4].GetSupervisor(), _employees[1]);
            Assert.AreEqual(_employees[5].GetSupervisor(),  _employees[2]);
            Assert.AreEqual(_employees[6].GetSupervisor(), _teamLead);
        }

        [Test]
        public void TestCreateEmployee()
        {
            Employee teamLead = _employees[0];
            Employee supervisor = _employees[1];
            Employee worker = _employees[3];

            Employee employee1 = new Worker("new_employee1");
            Employee employee2 = new Worker("new_employee2");
            Employee employee3 = new Worker("new_employee3");

            _employeeService.TryAddEmployee(employee1, teamLead);
            _employeeService.TryAddEmployee(employee2, supervisor);
            _employeeService.TryAddEmployee(employee3, worker);

            Assert.AreEqual(_employeeService.FindEmployeeById(employee1.Id), employee1);
            Assert.AreEqual(_employeeService.FindEmployeeById(employee2.Id), employee2);
            Employee employee = _employeeService.FindEmployeeById(employee3.Id);
            Assert.AreEqual(employee, default);
        }

        [Test]
        public void TestAddExistingEmployeeOrTeamLead()
        {
            Assert.That(!_employeeService.TryAddEmployee(_employees[0], _teamLead));
            var another = new TeamLead("another");
            Assert.That(!_employeeService.TryAddEmployee(another, _teamLead));
            Assert.That(!_employeeService.TryAddEmployee(_teamLead, _teamLead));
        }

        [Test]
        public void ReassignEmployee()
        {
            var worker = new Worker("worker");
            var supervisor = new Supervisor("supervisor");

            _employeeService.TryAddEmployee(worker);
            Assert.That(!_employeeService.TryAssignSupervisor(worker, supervisor));
            Assert.AreEqual(worker.GetSupervisor(), _teamLead);
            _employeeService.TryAddEmployee(supervisor);
            _employeeService.TryAssignSupervisor(worker, supervisor);
            Assert.AreEqual(worker.GetSupervisor(), supervisor);
        }

        [Test]
        public void TestRemoveEmployeeWithoutSubordinates()
        {
            var worker = new Worker("worker");
            _employeeService.TryAddEmployee(worker);
            Assert.That(_employeeService.TeamHasEmployee(worker));
            _employeeService.TryRemoveEmployee(worker);
            Assert.That(!_employeeService.TeamHasEmployee(worker));
        }

        [Test]
        public void TestRemoveEmployeeWithSubordinates()
        {
            Employee supervisor = _employees[0];
            List<Employee> children = _employeeService.GetEmployees(x => x.GetSupervisor() == supervisor);
            Employee supersupervisor = supervisor.GetSupervisor();
            Assert.That(_employeeService.TeamHasEmployee(supervisor));
            _employeeService.TryRemoveEmployee(supervisor);
            Assert.That(!_employeeService.TeamHasEmployee(supervisor));
            children.ForEach(x => Assert.AreEqual(x.GetSupervisor(), supersupervisor));
        }

        [Test]
        public void TestTryRemoveTeamLead()
        {
            Assert.That(!_employeeService.TryRemoveEmployee(_teamLead));
        }
    }
}