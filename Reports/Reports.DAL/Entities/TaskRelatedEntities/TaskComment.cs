using Reports.DAL.Entities.Employees;

namespace Reports.DAL.Entities.TaskRelatedEntities
{
    public class TaskComment : TaskModification
    {
        public TaskComment(Employee employee, string comment)
            : base(employee)
        {
            Comment = comment;
        }


        public string Comment { get; private set; }
        public override string GetNote()
        {
            return $"{DateCreated}\tEmployee {ChangeInitiator.Name} commented on task : {Comment}";
        }
    }
}