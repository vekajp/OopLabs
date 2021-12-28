using System.Collections.Generic;
using Reports.DAL.Entities;
using Reports.DAL.Entities.Employees;
using Reports.DAL.Entities.TaskRelatedEntities;
using Reports.Server.DataBase;

namespace Reports.Server.Services
{
    public interface IReportService : IEmployeeService, ITaskManager
    {
        bool Authorize(Employee employee);
        Employee GetAuthorized();
        List<Report> GetSubordinatesDailyReports();
        List<Employee> GetEmployeesNotReported();
        Report ComposeWeeklyReport();
        Report SendFinalReport();
        bool AddTaskToReport(Task task);
        List<Employee> GetSubordinatesOfCurrent();
        bool CreateWeeklyReport();
        Report GetWeeklyReport();
    }
}