using Reports.DAL.Entities.Employees;

namespace Reports.DAL.Entities.TaskRelatedEntities
{
    public class ChangeTaskState : TaskModification
    {
        public ChangeTaskState(Employee employee, TaskState state)
            : base(employee)
        {
            Comment = $"{DateCreated}\tEmployee {ChangeInitiator.Name} changed task state to {state}";
            State = state;
        }

        public TaskState State { get; init; }
    }
}