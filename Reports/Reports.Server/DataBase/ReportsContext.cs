using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using Reports.DAL.Entities;
using Reports.DAL.Entities.Employees;
using Reports.DAL.Entities.TaskRelatedEntities;
using Reports.Server.DataBase.EntitiesDto;
using Reports.Server.Services;

namespace Reports.Server.DataBase
{
    public sealed class ReportsContext : DbContext
    {
        public ReportsContext(DbContextOptions<ReportsContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<TaskDto> Tasks { get; set; }
        public DbSet<ReportDto> Reports { get; set; }
        public DbSet<EmployeeDto> Employees { get; set; }
        public DbSet<TaskModificationDto> TaskModifications { get; set; }

        public IEmployeeService LastLoadedEmployeeService { get; set; }
        public ITaskManager LastLoadedTaskManager { get; set; }

        public IReportService RestoreReportsService()
        {
            LastLoadedEmployeeService = RestoreEmployeeService();
            LastLoadedTaskManager = RestoreTaskManager(LastLoadedEmployeeService);
            return RestoreReportsService(LastLoadedTaskManager, LastLoadedEmployeeService);
        }

        public void UpdateEmployeesDatabase(IEmployeeService employeeService)
        {
            List<Employee> employees = employeeService.GetAllEmployees();

            // Add new employees
            foreach (Employee employee in employees.Where(x => !Employees.Any(y => y.Id == x.Id)))
            {
                Employees.Add(new EmployeeDto(employee));
                Employees.ToList().Find(x => employee.GetSupervisorId() == x.Id)?.SubordinatesGuids.Add(employee.Id);
            }

            // Update data in existing employees
            employees.Where(x => Employees.ToList().Any(y => y.Id == x.Id)).ToList().ForEach(x => Employees.ToList().Find(y => y.Id == x.Id)?.Update(x));

            // Remove deleted employees
            Employees.ToList().Where(x => employees.All(y => y.Id != x.Id)).ToList().ForEach(x => Employees.Remove(x));

            // Add new reports
            var reports = employees.Select(x => x.DraftReport).ToList();
            reports.Where(x => !Reports.Any(y => y.Id == x.Id)).ToList().ForEach(x => Reports.Add(new ReportDto(x)));

            // Update existing reports
            reports.Where(x => Reports.ToList().Any(y => y.Id == x.Id)).ToList().ForEach(x => Reports.ToList().Find(y => x.Id == y.Id)?.Update(x));

            // Remove reports
            Reports.ToList().Where(x => reports.All(y => y.Id != x.Id)).ToList().ForEach(x => Reports.Remove(x));
        }

        public void UpdateTasksDatabase(ITaskManager manager)
        {
            // Add new tasks
            List<Task> tasks = manager.GetAllTasks();
            tasks.Where(x => Tasks.ToList().All(y => y.Id != x.Id)).ToList().ForEach(x => Tasks.Add(new TaskDto(x)));

            // Update existing tasks
            tasks.Where(x => Tasks.ToList().Any(y => y.Id == x.Id)).ToList().ForEach(x => Tasks.ToList().Find(y => x.Id == y.Id)?.Update(x));

            // Add comments
            var comments = tasks.SelectMany(x => x.Comments).ToList();
            comments.Where(x => !TaskModifications.Any(y => y.Id == x.Id)).ToList().ForEach(x => TaskModifications.Add(new TaskModificationDto(x)));

            // Remove comments
            TaskModifications.ToList().Where(x => comments.All(y => y.Id != x.Id)).ToList().ForEach(x => TaskModifications.Remove(x));
        }


        public void RemoveEmployee(Employee employee)
        {
            EmployeeDto dto = Employees.ToList().Find(x => x.Id == employee.Id);
            UpdateEmployee(employee.GetSupervisor());
            employee.GetSubordinates(x => x.GetSupervisor() == employee).ForEach(UpdateEmployee);
        }

        public void UpdateEmployee(Employee employee)
        {
           Employees.ToList().Find(x => x.Id == employee.Id)?.Update(employee);
        }

        public void AddEmployee(Employee employee, Employee supervisor)
        {
            Employees.Add(new EmployeeDto(employee));
            UpdateEmployee(supervisor);
        }

        public void AddTask(Task task)
        {
            Tasks.Add(new TaskDto(task));
            task.Comments.ForEach(x => TaskModifications.Add(new TaskModificationDto(x)));
        }

        public void RemoveTask(Task task)
        {
            TaskDto dto = Tasks.ToList().Find(x => x.Id == task.Id) ?? throw new DataException();
            Tasks.Remove(dto);
            dto.Comments.ForEach(x => TaskModifications.Remove(TaskModifications.ToList().Find(y => y.Id == x) ?? throw new DataException()));
        }

        public void UpdateTask(Task task)
        {
            Tasks.ToList().Find(x => x.Id == task.Id)?.Update(task);
        }

        public void UpdateReport(Report report)
        {
            ReportDto dto = Reports.ToList().Find(x => x.Id == report.Id);
            if (dto == null)
            {
                Reports.Add(new ReportDto(report));
            }
            else
            {
                dto.Update(report);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var converter = new ValueConverter<List<Guid>, string>(
                x => JsonConvert.SerializeObject(x),
                x => JsonConvert.DeserializeObject<List<Guid>>(x));

            modelBuilder.Entity<EmployeeDto>()
                .Property(x => x.SubordinatesGuids)
                .HasConversion<string>(converter);

            modelBuilder.Entity<ReportDto>()
                .Property(x => x.ReportsIds)
                .HasConversion(converter);
            modelBuilder.Entity<ReportDto>()
                .Property(x => x.TasksIds)
                .HasConversion(converter);

            modelBuilder.Entity<TaskDto>()
                .Property(x => x.Comments)
                .HasConversion(converter);
            base.OnModelCreating(modelBuilder);
        }

        private IEmployeeService RestoreEmployeeService()
        {
            EmployeeDto teamLeadDto = Employees.FirstOrDefault(x => x.Type == EmployeeType.TeamLead) ?? throw new DataException("Team lead was not found in database");
            TeamLead teamLead = teamLeadDto?.RestoreEmployee() as TeamLead ?? throw new InvalidCastException(nameof(teamLeadDto));
            IEmployeeService service = new EmployeeService(teamLead);
            teamLeadDto.SubordinatesGuids.ForEach(x => RecursiveAddEmployees(Employees.ToList().Find(d => d.Id == x), teamLead, service));
            return service;
        }

        private void RecursiveAddEmployees(EmployeeDto current, Employee supervisor, IEmployeeService service)
        {
            Employee employee = current.RestoreEmployee();
            service.TryAddEmployee(employee, supervisor);
            service.TryAssignSupervisor(employee, supervisor);
            current.SubordinatesGuids
                .ForEach(x => RecursiveAddEmployees(Employees.ToList().Find(d => d.Id == x), employee, service));
        }

        private ITaskManager RestoreTaskManager(IEmployeeService employeeService)
        {
            ITaskManager taskManager = new TaskManager();
            foreach (TaskDto taskDto in Tasks)
            {
                List<Guid> comments = taskDto.Comments;
                Task task = taskDto.RestoreTask(employeeService);
                task.Comments = TaskModifications.Where(x => comments.Any(y => y == x.Id))
                    .Select(x => x.RestoreModification(employeeService)).ToList();
                taskManager.AddTask(task);
            }

            return taskManager;
        }

        private IReportService RestoreReportsService(ITaskManager taskManager, IEmployeeService employeeService)
        {
            IReportService reportService = new ReportService(employeeService, taskManager);
            var restored = Reports.Select(x => x.RestoreReport(employeeService, taskManager)).ToList();
            foreach (ReportDto reportDto in Reports)
            {
                Report restoredReport = restored.Find(x => x.Id == reportDto.Id) ?? throw new DataException(nameof(reportDto));
                var reports = reportDto.ReportsIds.Select(x => restored.Find(report => report.Id == x)).ToList();
                restoredReport.AddReports(reports);
            }

            return reportService;
        }
    }
}