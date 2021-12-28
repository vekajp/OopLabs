using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Reports.DAL.Entities.TaskRelatedEntities;

namespace Reports.DAL.Entities.Employees
{
    public abstract class Employee
    {
        protected Employee(string name)
        {
            Name = name;
            Id = Guid.NewGuid();
            DraftReport = new Report(this);
        }

        public Guid Id { get; init; }
        public string Name { get; init; }

        [NotMapped]
        public Report DraftReport { get; set; }

        [NotMapped]
        protected Employee Supervisor { get; private set; }
        protected virtual List<Employee> Employees { get; set; } = new List<Employee>();

        public virtual List<Employee> GetSubordinates(Predicate<Employee> predicate)
        {
            var employees = new List<Employee>();
            Employees.Where(x => predicate(x)).ToList().ForEach(x => employees.Add(x));
            Employees.SelectMany(x => x.GetSubordinates(predicate)).ToList().ForEach(x => employees.Add(x));
            return employees;
        }

        public abstract bool AddSubordinate(Employee employee);
        public abstract bool RemoveSubordinate(Employee employee);
        public void ReassignEmployee(Employee employee)
        {
            Supervisor?.RemoveSubordinate(this);
            Supervisor = employee;
            employee.AddSubordinate(this);
        }

        public Employee GetSupervisor()
        {
            return Supervisor;
        }

        public Report IncludeTasksToReport(List<Task> tasks)
        {
            DraftReport.AddTasks(tasks);
            return DraftReport;
        }

        public Report AddTaskToReport(Task task)
        {
            DraftReport.AddTask(task);
            return DraftReport;
        }

        public Report IncludeReports(List<Report> reports)
        {
            DraftReport.AddReports(reports);
            return DraftReport;
        }

        public Report IncludeReport(Report report)
        {
            DraftReport.AddReport(report);
            return DraftReport;
        }

        public Report GetDraftReport()
        {
            return DraftReport;
        }

        public virtual Guid GetSupervisorId()
        {
            return Supervisor.Id;
        }

        public abstract Report SendFinalReport();
        protected Report GetFinalReport()
        {
            return DraftReport;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != GetType()) return false;
            var other = (Employee)obj;
            return other.Id == Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public void ClearReport()
        {
            DraftReport = new Report(this);
        }
    }
}