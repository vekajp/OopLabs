using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Reports.DAL.Entities.Employees;
using Reports.DAL.Entities.TaskRelatedEntities;
using Reports.Server.DataBase;
using Reports.Server.Services;

namespace Reports.Tests
{
    public class DbTests
    {
        private IEmployeeService _employeeService;
        private IReportService _reportService;
        private ITaskManager _taskManager;
        private Employee _testWorker;
        [SetUp]
        public void SetUp()
        {
            // Create employees
            var teamLead = new TeamLead("teamlead");
            _employeeService = new EmployeeService(teamLead);
            var supervisor = new Supervisor("supervisor1");
            var worker1 = new Worker("worker1");
            _testWorker = new Worker("worker2");
            _employeeService.TryAddEmployee(supervisor);
            _employeeService.TryAddEmployee(worker1);
            _employeeService.TryAddEmployee(_testWorker, supervisor);

            // Create tasks
            _taskManager = new TaskManager();
            var task1 = new Task("task1", teamLead);
            var task2 = new Task("task2", supervisor);
            var task3 = new Task("task3", _testWorker);

            _taskManager.AddTask(task1);
            _taskManager.AddTask(task2);
            _taskManager.AddTask(task3);

            task1.LeaveComment(_testWorker, "blabla");
            task2.ChangeState(supervisor, TaskState.Active);
            task2.AssignToEmployee(supervisor, _testWorker);
            task2.ChangeState(_testWorker, TaskState.Resolved);
            task3.AssignToEmployee(teamLead, worker1);

            // Create draft reports
            _reportService = new ReportService(_employeeService, _taskManager);
            _reportService.Authorize(worker1);
            _reportService.ComposeWeeklyReport();
            _reportService.SendFinalReport();

            _reportService.Authorize(_testWorker);
            _reportService.ComposeWeeklyReport();
            _reportService.SendFinalReport();

            _reportService.Authorize(supervisor);
            _reportService.ComposeWeeklyReport();
            _reportService.SendFinalReport();

            _reportService.Authorize(teamLead);
            _reportService.ComposeWeeklyReport();
        }

        [Test]
        public void TestLoadAndSave()
        {
            DbContextOptionsBuilder<ReportsContext> opt =
                new DbContextOptionsBuilder<ReportsContext>().UseInMemoryDatabase("database1");
            var context = new ReportsContext(opt.Options);
            context.UpdateEmployeesDatabase(_employeeService);
            context.UpdateTasksDatabase(_taskManager);
            context.SaveChanges();
            IReportService loadedReportService = context.RestoreReportsService();
            IEmployeeService loadedEmployeeService = context.LastLoadedEmployeeService;
            ITaskManager loadedTaskManager = context.LastLoadedTaskManager;

            Assert.AreEqual(loadedEmployeeService.GetAllEmployees().Count, _employeeService.GetAllEmployees().Count);
            Assert.AreEqual(loadedTaskManager.GetAllTasks().Count, _taskManager.GetAllTasks().Count);
        }

        [Test]
        public void TestAddAndRemoveEmployee()
        {
            DbContextOptionsBuilder<ReportsContext> opt =
                new DbContextOptionsBuilder<ReportsContext>().UseInMemoryDatabase("database2");
            var context = new ReportsContext(opt.Options);
            Employee employee = new Supervisor("supervisor2");
            _employeeService.TryAddEmployee(employee);
            context.UpdateEmployeesDatabase(_employeeService);
            context.UpdateTasksDatabase(_taskManager);
            context.SaveChanges();
            context.RestoreReportsService();
            IEmployeeService loadedEmployeeService = context.LastLoadedEmployeeService;
            ITaskManager loadedTaskManager = context.LastLoadedTaskManager;

            Assert.AreEqual(loadedEmployeeService.GetAllEmployees().Count, _employeeService.GetAllEmployees().Count);
            Assert.AreEqual(loadedTaskManager.GetAllTasks().Count, _taskManager.GetAllTasks().Count);
            Employee teamLead = loadedEmployeeService.GetTeamLead();
            Assert.AreEqual(teamLead.GetSubordinates(x => x.GetSupervisor() == teamLead).Count, 3);
            Assert.AreEqual(loadedEmployeeService.GetAllEmployees().Count, _employeeService.GetAllEmployees().Count);

            _employeeService.TryRemoveEmployee(employee);
            context.UpdateEmployeesDatabase(_employeeService);
            context.SaveChanges();
            context.RestoreReportsService();
            loadedEmployeeService = context.LastLoadedEmployeeService;
            teamLead = loadedEmployeeService.GetTeamLead();
            Assert.AreEqual(teamLead.GetSubordinates(x => x.GetSupervisor() == teamLead).Count, 2);
            Assert.AreEqual(loadedEmployeeService.GetAllEmployees().Count, _employeeService.GetAllEmployees().Count);
        }

        [Test]
        public void AddAndRemoveTasksAndModifications()
        {
            DbContextOptionsBuilder<ReportsContext> opt =
                new DbContextOptionsBuilder<ReportsContext>().UseInMemoryDatabase("database3");
            var context = new ReportsContext(opt.Options);
            context.UpdateEmployeesDatabase(_employeeService);
            context.UpdateTasksDatabase(_taskManager);
            context.SaveChanges();
            var task = new Task("new task", _testWorker);
            _taskManager.AddTask(task);

            context.UpdateTasksDatabase(_taskManager);
            context.SaveChanges();
            context.RestoreReportsService();
            Assert.AreEqual(context.LastLoadedTaskManager.GetAllTasks().Count, _taskManager.GetAllTasks().Count);

            task.LeaveComment(_testWorker, "hihi");
            task.ChangeState(_testWorker, TaskState.Resolved);
            context.UpdateTasksDatabase(_taskManager);
            context.SaveChanges();
            context.RestoreReportsService();
            Assert.AreEqual(context.LastLoadedTaskManager.GetAllTasks().Find(x => x.Id == task.Id)?.Comments.Count, task.Comments.Count);
            Assert.AreEqual(context.LastLoadedTaskManager.GetAllTasks().Find(x => x.Id == task.Id)?.State, task.State);
        }
    }
}