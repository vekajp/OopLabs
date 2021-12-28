using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Reports.DAL.Entities.Employees;

namespace Reports.DAL.Entities.TaskRelatedEntities
{
    public class Task
    {
        public Task(string name, Employee creator)
        {
            Comments = new List<TaskModification>();
            Id = Guid.NewGuid();
            Name = name;
            DateCreated = DateTime.Now;
            ChangeState(creator, TaskState.Open);
            PinedEmployee = creator;
        }

        public Guid Id { get; init; }
        public string Name { get; private set; }
        public TaskState State { get; set; }
        [JsonIgnore]
        public List<TaskModification> Comments { get; set; }
        [JsonIgnore]
        public Employee PinedEmployee { get; set; }
        public DateTime DateCreated { get; init; }
        public DateTime DateModified { get; private set; }
        [JsonIgnore]
        public Employee Creator => Comments.First().ChangeInitiator;

        public Task AssignToEmployee(Employee initiator, Employee another)
        {
            var comment = new ReassignTask(initiator, PinedEmployee, another);
            Comments.Add(comment);
            PinedEmployee = another;
            DateModified = comment.DateCreated;
            return this;
        }

        public Task ChangeState(Employee initiator, TaskState state)
        {
            if (state < State)
            {
                throw new ArgumentException("Cannot downgrade task state", nameof(state));
            }

            var comment = new ChangeTaskState(initiator, state);
            Comments.Add(comment);
            State = state;
            DateModified = comment.DateCreated;
            return this;
        }

        public Task LeaveComment(Employee employee, string comment)
        {
            var taskComment = new TaskComment(employee, comment);
            Comments.Add(taskComment);
            DateModified = taskComment.DateCreated;
            return this;
        }
    }
}