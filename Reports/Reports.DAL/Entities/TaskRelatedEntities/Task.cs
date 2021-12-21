using System;
using System.Collections.Generic;
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

        public Guid Id { get; private init; }
        public string Name { get; private set; }
        public TaskState State { get; private set; }
        public List<TaskModification> Comments { get; private set; }
        public Employee PinedEmployee { get; private set; }
        public DateTime DateCreated { get; private init; }
        public DateTime DateModified { get; private set; }

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