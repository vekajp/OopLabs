using System;
using Reports.DAL.Entities.Employees;

namespace Reports.DAL.Entities.TaskRelatedEntities
{
    public abstract class TaskModification
    {
        protected TaskModification(Employee employee)
        {
            ChangeInitiator = employee;
            DateCreated = DateTime.Now;
        }

        public Employee ChangeInitiator { get; private init; }
        public DateTime DateCreated { get; private set; }
        public abstract string GetNote();
    }
}