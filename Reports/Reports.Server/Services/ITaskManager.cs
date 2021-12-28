using System;
using System.Collections.Generic;
using Reports.DAL.Entities;
using Reports.DAL.Entities.Employees;
using Reports.DAL.Entities.TaskRelatedEntities;
using Reports.Server.DataBase.EntitiesDto;

namespace Reports.Server.Services
{
    public interface ITaskManager
    {
        Task FindTaskById(Guid id);
        List<Task> GetAllTasks();
        List<Task> GetTasksByDateCreated(DateTime dateCreated);
        List<Task> GetTasksByDateLastModified(DateTime dateModified);
        List<Task> GetTasksByEmployee(Employee employee);
        List<Task> GetTasksModifiedByEmployee(Employee employee);
        List<Task> GetTasksByEmployeeSubordinates(Employee employee);
        List<Task> GetTasksOfWeek(DateTime current);
        bool AddTask(Task task);
        bool RemoveTask(Task task);
        bool TryChangeTaskState(Task task, TaskState newState, Employee initiator);
        bool TryReassignToEmployee(Task task, Employee newEmployee, Employee initiator);
        bool TryCommentOnTask(Task task, string comment, Employee initiator);
    }
}