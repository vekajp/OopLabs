using System.Collections.Generic;
using Reports.DAL.Entities.TaskRelatedEntities;

namespace Reports.DAL.Entities.Employees
{
    public class Worker : Employee
    {
        public Worker(string name)
            : base(name)
        {
        }

        public override bool AddSubordinate(Employee employee)
        {
            return false;
        }

        public override bool RemoveSubordinate(Employee employee)
        {
            return false;
        }
    }
}