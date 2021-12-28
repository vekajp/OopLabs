using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Reports.DAL.Entities.Employees;
using Reports.DAL.Entities.TaskRelatedEntities;
using Reports.Server.Services;

namespace Reports.Server.DataBase.EntitiesDto
{
    public class TaskModificationDto
    {
        public TaskModificationDto()
        {
        }

        public TaskModificationDto(TaskModification modification)
        {
            Id = modification.Id;
            InitiatorGuid = modification.ChangeInitiator.Id;
            DateCreated = modification.DateCreated;
            Type = GetModificationType(modification);
            CollectExtraData(modification);
        }

        public Guid Id { get; set; }
        public Guid InitiatorGuid { get; set; }
        public DateTime DateCreated { get; set; }
        public string Comment { get; set; }
        public ModificationType Type { get; set; }
        public TaskState State { get; set; }
        public Guid OldId { get; set; }
        public Guid AssignedId { get; set; }

        public TaskModification RestoreModification(IEmployeeService service)
        {
            Employee initiator = service.FindEmployeeById(InitiatorGuid);
            switch (Type)
            {
                case ModificationType.ChangeState:
                    return new ChangeTaskState(initiator, State)
                    {
                        Id = Id,
                        DateCreated = DateCreated,
                    };
                case ModificationType.Reassign:
                    Employee old = service.FindEmployeeById(OldId);
                    Employee assigned = service.FindEmployeeById(AssignedId);
                    return new ReassignTask(initiator, old, assigned)
                    {
                        Id = Id,
                        DateCreated = DateCreated,
                    };
                case ModificationType.Comment:
                    return new TaskComment(initiator, Comment)
                    {
                        Id = Id,
                        DateCreated = DateCreated,
                    };
                case ModificationType.Create:
                    return new TaskCreate(initiator)
                    {
                        Id = Id,
                        DateCreated = DateCreated,
                    };
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private ModificationType GetModificationType(TaskModification modification)
        {
            if (modification.GetType() == typeof(TaskCreate))
            {
                return ModificationType.Create;
            }
            else if (modification.GetType() == typeof(TaskComment))
            {
                return ModificationType.Comment;
            }
            else if (modification.GetType() == typeof(ReassignTask))
            {
                return ModificationType.Reassign;
            }
            else if (modification.GetType() == typeof(ChangeTaskState))
            {
                return ModificationType.ChangeState;
            }

            throw new ArgumentOutOfRangeException(nameof(modification));
        }

        private void CollectExtraData(TaskModification modification)
        {
            if (Type == ModificationType.ChangeState)
            {
                ChangeTaskState changeTaskState = modification as ChangeTaskState ?? throw new InvalidCastException(nameof(modification));
                State = changeTaskState.State;
            }
            else if (Type == ModificationType.Reassign)
            {
                ReassignTask reassignTask = modification as ReassignTask ?? throw new InvalidCastException(nameof(modification));
                OldId = reassignTask.Old.Id;
                AssignedId = reassignTask.Assigned.Id;
            }
            else if (Type == ModificationType.Comment)
            {
                TaskComment taskComment = modification as TaskComment ?? throw new InvalidCastException(nameof(modification));
                Comment = taskComment.InnerComment;
            }
        }
    }
}