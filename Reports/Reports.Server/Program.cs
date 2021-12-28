using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Reports.DAL.Entities.Employees;
using Reports.DAL.Entities.TaskRelatedEntities;
using Reports.Server.DataBase;
using Reports.Server.Services;

namespace Reports.Server
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            // FillDataBase();
            // var opt = new DbContextOptionsBuilder<ReportsContext>();
            // opt.UseSqlite(@"Data Source=ReportsDB.db;");
            // var context = new ReportsContext(opt.Options);
            // var reportService = new ReportService(context);
            // var e1 = reportService.GetTeamLead();
            // var es = reportService.GetAllEmployees();
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
        private static void FillDataBase()
        {
            var teamLead = new TeamLead("teamlead");
            Employee supervisor1 = new Supervisor("supervisor1");
            Employee supervisor2 = new Supervisor("supervisor2");
            Employee supervisor3 = new Supervisor("supervisor3");
            var employees = new List<Employee>
            {
                supervisor1,
                supervisor2,
                supervisor3,
                new Worker("smart_worker"),
                new Worker("not_so_smart_worker"),
                new Worker("not_smart_worker"),
                new Worker("simple_worker"),
            };
            var employeeService = new EmployeeService(teamLead);

            employeeService.TryAddEmployee(supervisor1, teamLead);
            employeeService.TryAddEmployee(supervisor2, teamLead);
            employeeService.TryAddEmployee(supervisor3, supervisor1);
            employeeService.TryAddEmployee(employees[3], supervisor1);
            employeeService.TryAddEmployee(employees[4], supervisor2);
            employeeService.TryAddEmployee(employees[5], supervisor3);

            var manager = new TaskManager();
            manager.AddTask(new Task("task1", employees[0]));
            manager.AddTask(new Task("task2", employees[1]));
            manager.AddTask(new Task("task3", employees[2]));
            manager.AddTask(new Task("task4", employees[3]));
            manager.AddTask(new Task("task5", employees[4]));

            var opt = new DbContextOptionsBuilder<ReportsContext>();
            opt.UseSqlite(@"Data Source=ReportsDB.db;");
            var context = new ReportsContext(opt.Options);
            context.UpdateEmployeesDatabase(employeeService);
            context.UpdateTasksDatabase(manager);
            context.SaveChanges();
        }
    }
}