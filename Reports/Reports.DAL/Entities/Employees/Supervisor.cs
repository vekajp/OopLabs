using System.Collections.Generic;
using Reports.DAL.Entities.TaskRelatedEntities;

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
    }
}