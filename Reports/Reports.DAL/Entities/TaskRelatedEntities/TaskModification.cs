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
            Id = Guid.NewGuid();
        }

        public Guid Id { get; init; }
        public Employee ChangeInitiator { get; init; }
        public DateTime DateCreated { get; init; }
        public string Comment { get; protected set; }
        public string GetNote()
        {
            return Comment;
        }
    }
}