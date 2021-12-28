using System;
using System.Collections.Generic;
using System.Linq;
using Reports.DAL.Entities;
using Reports.DAL.Entities.Employees;
using Reports.Server.Services;

namespace Reports.Server.DataBase.EntitiesDto
{
    public class ReportDto
    {
        public ReportDto()
        {
        }

        public ReportDto(Report report)
        {
            Id = report.Id;
            AuthorId = report.Author.Id;
            TasksIds = report.Tasks.Select(x => x.Id).ToList();
            ReportsIds = report.Reports.Select(x => x.Id).ToList();
        }

        public Guid Id { get; set; }
        public Guid AuthorId { get; private set; }
        public virtual List<Guid> TasksIds { get; set; }
        public virtual List<Guid> ReportsIds { get; set; }
        public Report RestoreReport(IEmployeeService employeeService, ITaskManager taskManager)
        {
            var tasks = TasksIds.Select(x => taskManager.FindTaskById(x)).ToList();
            Employee creator = employeeService.FindEmployeeById(AuthorId);
            var report = new Report(creator)
            {
                Id = Id,
            };

            creator.DraftReport = report;
            return report;
        }

        public void Update(Report report)
        {
            TasksIds = report.Tasks.Select(x => x.Id).ToList();
            ReportsIds = report.Reports.Select(x => x.Id).ToList();
        }
    }
}