using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Reports.DAL.Entities;
using Reports.DAL.Entities.Employees;
using Reports.DAL.Entities.TaskRelatedEntities;
using Reports.Server.DataBase;
using Task = Reports.DAL.Entities.TaskRelatedEntities.Task;

namespace Reports.Server.Services
{
    public class ReportService : IReportService, IDisposable
    {
        private IEmployeeService _employeeService;
        private ITaskManager _taskManager;
        private TeamLead _teamLead;
        private Employee _authorized;
        private ReportsContext _context;
        public ReportService(ReportsContext context)
        {
            _context = context;

            // Restore service from database
            _context.RestoreReportsService();
            _employeeService = _context.LastLoadedEmployeeService;
            _taskManager = _context.LastLoadedTaskManager;
            _teamLead = GetTeamLead() as TeamLead;
            _authorized = _teamLead;
        }

        public ReportService(IEmployeeService service, ITaskManager manager)
        {
            _employeeService = service;
            _taskManager = manager;
            _teamLead = GetTeamLead() as TeamLead;
            _authorized = _teamLead;
        }

        public Employee GetTeamLead()
        {
            return _employeeService.GetTeamLead();
        }

        public List<Employee> GetAllEmployees()
        {
            return _employeeService.GetAllEmployees();
        }

        public List<Employee> GetEmployees(Predicate<Employee> condition)
        {
            return _employeeService.GetEmployees(condition);
        }

        public Employee FindEmployeeById(Guid id)
        {
            return _employeeService.FindEmployeeById(id);
        }

        public Employee FindEmployeeByName(string name)
        {
            return _employeeService.FindEmployeeByName(name);
        }

        public bool TryRemoveEmployee(Employee employee)
        {
            bool result = _employeeService.TryRemoveEmployee(employee);
            if (result)
            {
                _context?.RemoveEmployee(employee);
                _context?.SaveChanges();
            }

            return result;
        }

        public bool TryAddEmployee(Employee subordinate, Employee supervisor)
        {
            bool result = _employeeService.TryAddEmployee(subordinate, supervisor);
            if (result)
            {
                _context?.AddEmployee(subordinate, supervisor);
                _context?.SaveChanges();
            }

            return result;
        }

        public bool TryAddEmployee(Employee subordinate)
        {
            return TryAddEmployee(subordinate, GetTeamLead());
        }

        public bool TeamHasEmployee(Employee employee)
        {
            return _employeeService.TeamHasEmployee(employee);
        }

        public bool TryAssignSupervisor(Employee subordinate, Employee supervisor)
        {
            if (!_employeeService.TryAssignSupervisor(subordinate, supervisor)) return false;
            _context?.UpdateEmployee(subordinate);
            _context?.UpdateEmployee(supervisor);
            _context?.SaveChanges();
            return true;
        }

        public Task FindTaskById(Guid id)
        {
            return _taskManager.FindTaskById(id);
        }

        public List<Task> GetAllTasks()
        {
            return _taskManager.GetAllTasks();
        }

        public List<Task> GetTasksByDateCreated(DateTime dateCreated)
        {
            return _taskManager.GetTasksByDateCreated(dateCreated);
        }

        public List<Task> GetTasksByDateLastModified(DateTime dateModified)
        {
            return _taskManager.GetTasksByDateLastModified(dateModified);
        }

        public List<Task> GetTasksByEmployee(Employee employee)
        {
            return _taskManager.GetTasksByEmployee(employee);
        }

        public List<Task> GetTasksModifiedByEmployee(Employee employee)
        {
            return _taskManager.GetTasksModifiedByEmployee(employee);
        }

        public List<Task> GetTasksByEmployeeSubordinates(Employee employee)
        {
            return _taskManager.GetTasksByEmployeeSubordinates(employee);
        }

        public List<Task> GetTasksOfWeek(DateTime current)
        {
            return _taskManager.GetTasksOfWeek(current);
        }

        public bool AddTask(Task task)
        {
            bool result = _taskManager.AddTask(task);
            if (result)
            {
                _context?.AddTask(task);
                _context?.SaveChanges();
            }

            return result;
        }

        public bool RemoveTask(Task task)
        {
            bool result = _taskManager.RemoveTask(task);
            if (result)
            {
                _context?.RemoveTask(task);
                _context?.SaveChanges();
            }

            return result;
        }

        public bool TryChangeTaskState(Task task, TaskState newState, Employee initiator)
        {
            return _taskManager.TryChangeTaskState(task, newState, initiator);
        }

        public bool TryReassignToEmployee(Task task, Employee newEmployee, Employee initiator)
        {
            return _taskManager.TryReassignToEmployee(task, newEmployee, initiator);
        }

        public bool TryCommentOnTask(Task task, string comment, Employee initiator)
        {
            return _taskManager.TryCommentOnTask(task, comment, initiator);
        }

        public bool Authorize(Employee employee)
        {
            _authorized = employee ?? throw new ArgumentNullException(nameof(employee));
            return true;
        }

        public Employee GetAuthorized()
        {
            return _authorized;
        }

        public List<Report> GetSubordinatesDailyReports()
        {
            List<Employee> subordinates = GetSubordinatesOfCurrent();
            return subordinates.Select(x => x.GetDraftReport()).ToList();
        }

        public List<Employee> GetEmployeesNotReported()
        {
            return GetSubordinatesOfCurrent()
                .Where(x => _authorized.GetDraftReport().Reports.All(y => !Equals(y.Author, x))).ToList();
        }

        public Report ComposeWeeklyReport()
        {
            List<Task> tasks = GetTasksByEmployee(_authorized);
            _authorized.IncludeTasksToReport(tasks);
            _context?.UpdateEmployeesDatabase(_employeeService);
            _context?.SaveChanges();
            return _authorized.GetDraftReport();
        }

        public Report SendFinalReport()
        {
            Report final = _authorized.SendFinalReport();
            _context?.UpdateEmployeesDatabase(_employeeService);
            _context?.SaveChanges();
            _authorized.DraftReport = new Report(_authorized);
            return final;
        }

        public bool AddTaskToReport(Task task)
        {
            _authorized.AddTaskToReport(task);
            _context?.UpdateEmployeesDatabase(_employeeService);
            _context?.SaveChanges();
            return true;
        }

        public List<Employee> GetSubordinatesOfCurrent()
        {
            _ = _authorized ?? throw new NullReferenceException("No employee has authorized yet");
            return _employeeService.GetEmployees(x => Equals(x.GetSupervisor(), _authorized));
        }

        public bool CreateWeeklyReport()
        {
            _teamLead.SendFinalReport();
            _context?.UpdateEmployeesDatabase(_employeeService);
            _context?.SaveChanges();
            return true;
        }

        public Report GetWeeklyReport()
        {
            return _teamLead.SendFinalReport();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}