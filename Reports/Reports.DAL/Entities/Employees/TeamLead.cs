using System;
using System.Collections.Generic;
using System.Linq;
using Reports.DAL.Tools;

namespace Reports.DAL.Entities.Employees
{
    public class TeamLead : Employee
    {
        public TeamLead(string name)
            : base(name)
        {
        }

        public override bool AddSubordinate(Employee employee)
        {
            if (Employees.Contains(employee))
            {
                return false;
            }

            Employees.Add(employee);
            return true;
        }

        public override bool RemoveSubordinate(Employee employee)
        {
            List<Employee> subordinates = employee.GetSubordinates(x => true);
            bool success = Employees.Remove(employee);
            if (subordinates.Count > 0 && success)
            {
                subordinates.ForEach(x => x.ReassignEmployee(employee.GetSupervisor()));
            }

            return success;
        }

        public override Guid GetSupervisorId()
        {
            return Guid.Empty;
        }

        public Employee GetEmployeeById(Guid id)
        {
            return Id == id ? this : GetSubordinates(x => x.Id == id).FirstOrDefault();
        }

        public override List<Employee> GetSubordinates(Predicate<Employee> predicate)
        {
            var employees = new List<Employee>();
            Employees.Where(x => predicate(x)).ToList().ForEach(x => employees.Add(x));
            Employees.SelectMany(x => x.GetSubordinates(predicate)).ToList().ForEach(x => employees.Add(x));
            if (predicate(this))
            {
                employees.Add(this);
            }

            return employees;
        }

        public override Report SendFinalReport()
        {
            if (Employees.Any(x => DraftReport.Reports.Select(y => y.Author).All(y => !Equals(y, x))))
            {
                throw new ReportException("Not all employees have submitted their reports");
            }

            return GetFinalReport();
        }
    }
}