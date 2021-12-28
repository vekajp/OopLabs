using System.Collections.Generic;
using System.Linq;
using Reports.DAL.Tools;

namespace Reports.DAL.Entities.Employees
{
    public class Supervisor : Employee
    {
        public Supervisor(string name)
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

        public override Report SendFinalReport()
        {
            if (Employees.Any(x => DraftReport.Reports.Select(y => y.Author).All(y => !Equals(y, x))))
            {
                throw new ReportException("Not all employees have submitted their reports");
            }

            Supervisor.IncludeReport(GetFinalReport());
            return DraftReport;
        }
    }
}