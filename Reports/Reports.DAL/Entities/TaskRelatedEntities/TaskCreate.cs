using Reports.DAL.Entities.Employees;

namespace Reports.DAL.Entities.TaskRelatedEntities
{
    public class TaskCreate : TaskModification
    {
        public TaskCreate(Employee employee)
            : base(employee)
        {
            Comment = $"{DateCreated}\tTask was created by employee {ChangeInitiator.Name}";
        }
    }
}