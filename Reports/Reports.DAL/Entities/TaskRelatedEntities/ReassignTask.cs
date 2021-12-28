using Reports.DAL.Entities.Employees;

namespace Reports.DAL.Entities.TaskRelatedEntities
{
    public class ReassignTask : TaskModification
    {
        public ReassignTask(Employee initiator, Employee old, Employee assigned)
            : base(initiator)
        {
            Comment = $"{DateCreated}\t{ChangeInitiator.Name} reassigned task from {old.Name} to {assigned.Name}";
            Old = old;
            Assigned = assigned;
        }

        public Employee Old { get; init; }
        public Employee Assigned { get; init; }
    }
}