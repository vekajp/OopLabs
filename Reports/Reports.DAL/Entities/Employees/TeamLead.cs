using System;
using System.Collections.Generic;
using System.Linq;

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

        public Employee GetEmployeeById(Guid id)
        {
            return Id == id ? this : GetSubordinates(x => x.Id == id).FirstOrDefault();
        }

        public List<Employee> GetSubordinates(Predicate<Employee> predicate)
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
    }
}