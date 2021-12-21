using Reports.DAL.Entities.Employees;

namespace Reports.DAL.Entities.TaskRelatedEntities
{
    public class ChangeTaskState : TaskModification
    {
        public ChangeTaskState(Employee employee, TaskState state)
            : base(employee)
        {
        }

        private TaskState State { get; set; }
        public override string GetNote()
        {
            return $"{DateCreated}\tEmployee {ChangeInitiator.Name} changed task state to {State}";
        }
    }
}