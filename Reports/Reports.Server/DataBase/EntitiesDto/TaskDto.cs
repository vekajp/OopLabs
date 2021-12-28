using System;
using System.Collections.Generic;
using System.Linq;
using Reports.DAL.Entities.Employees;
using Reports.DAL.Entities.TaskRelatedEntities;
using Reports.Server.Services;

namespace Reports.Server.DataBase.EntitiesDto
{
    public class TaskDto
    {
        public TaskDto()
        {
        }

        public TaskDto(Task task)
        {
            Id = task.Id;
            Name = task.Name;
            State = task.State;
            Comments = task.Comments.Select(x => x.Id).ToList();
            PinedEmployeeGuid = task.PinedEmployee.Id;
            DateCreated = task.DateCreated;
            CreatorId = task.Creator.Id;
        }

        public Guid Id { get; set; }
        public Guid CreatorId { get; set; }
        public string Name { get; set; }
        public TaskState State { get; set; }
        public virtual List<Guid> Comments { get; private set; }
        public Guid PinedEmployeeGuid { get; private set; }
        public DateTime DateCreated { get; private init; }

        public void Update(Task task)
        {
            State = task.State;
            Comments = task.Comments.Select(x => x.Id).ToList();
            PinedEmployeeGuid = task.PinedEmployee.Id;
        }

        public Task RestoreTask(IEmployeeService service)
        {
            Employee creator = service.FindEmployeeById(CreatorId);
            var task = new Task(Name, creator)
            {
                Id = Id,
                State = State,
                PinedEmployee = service.FindEmployeeById(PinedEmployeeGuid),
                DateCreated = DateCreated,
            };
            return task;
        }
    }
}