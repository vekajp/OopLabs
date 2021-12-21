using Reports.DAL.Entities.Employees;

namespace Reports.DAL.Entities.TaskRelatedEntities
{
    public class ReassignTask : TaskModification
    {
        public ReassignTask(Employee initiator, Employee old, Employee assigned)
            : base(initiator)
        {
            Old = old;
            Assigned = assigned;
        }

        private Employee Old { get; set; }
        private Employee Assigned { get; set; }
        public override string GetNote()
        {
            return $"{DateCreated}\t{ChangeInitiator.Name} reassigned task from {Old.Name} to {Assigned.Name}";
        }
    }
}