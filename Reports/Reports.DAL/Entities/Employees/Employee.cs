using System;
using System.Collections.Generic;
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
            Employees = new List<Employee>();
            DraftReport = new Report();
        }

        public Guid Id { get; private init; }
        public string Name { get; private init; }
        protected List<Employee> Employees { get; set; }
        public abstract bool AddSubordinate(Employee employee);
        public abstract bool RemoveSubordinate(Employee employee);
        private Employee Supervisor { get; set; }
        private Report DraftReport { get; set; }
        private Report LastReport { get; set; }
        public virtual List<Employee> GetSubordinates(Predicate<Employee> predicate)
        {
            var employees = new List<Employee>();
            Employees.Where(x => predicate(x)).ToList().ForEach(x => employees.Add(x));
            Employees.SelectMany(x => x.GetSubordinates(predicate)).ToList().ForEach(x => employees.Add(x));
            return employees;
        }

        public void ReassignEmployee(Employee employee)
        {
            Supervisor?.RemoveSubordinate(this);
            Supervisor = employee;
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

        public Report IncludeReports(List<Report> reports)
        {
            DraftReport.AddReports(reports);
            return DraftReport;
        }

        public Report GetDraftReport()
        {
            return DraftReport;
        }

        public Report GetFinalReport()
        {
            LastReport = DraftReport;
            DraftReport = new Report();
            return LastReport;
        }
    }
}