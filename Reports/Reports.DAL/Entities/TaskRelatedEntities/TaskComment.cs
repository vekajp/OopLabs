using Reports.DAL.Entities.Employees;

namespace Reports.DAL.Entities.TaskRelatedEntities
{
    public class TaskComment : TaskModification
    {
        public TaskComment(Employee employee, string comment)
            : base(employee)
        {
            Comment = $"{DateCreated}\tEmployee {ChangeInitiator.Name} commented on task : {comment}";
            InnerComment = comment;
        }


        public string InnerComment { get; init; }
    }
}