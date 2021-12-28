using System;
using System.Collections.Generic;
using System.Linq;
using Reports.DAL.Entities;
using Reports.DAL.Entities.Employees;
using Reports.DAL.Entities.TaskRelatedEntities;

namespace Reports.Server.Services
{
    public class TaskManager : ITaskManager
    {
        private readonly List<Task> _tasks;
        public TaskManager()
        {
            _tasks = new List<Task>();
        }

        public Task FindTaskById(Guid id)
        {
            return _tasks.Find(x => x.Id == id);
        }

        public List<Task> GetAllTasks()
        {
            return _tasks;
        }

        public List<Task> GetTasksByDateCreated(DateTime dateCreated)
        {
            return _tasks.Where(x => SameDay(x.DateCreated, dateCreated)).Select(x => x).ToList();
        }

        public List<Task> GetTasksByDateLastModified(DateTime dateModified)
        {
            return _tasks.Where(x => SameDay(x.DateModified, dateModified)).ToList();
        }

        public List<Task> GetTasksByEmployee(Employee employee)
        {
            return _tasks.Where(x => Equals(x.PinedEmployee, employee)).ToList();
        }

        public List<Task> GetTasksModifiedByEmployee(Employee employee)
        {
            return _tasks.Where(x => x.Comments.Any(modification => Equals(modification.ChangeInitiator, employee))).ToList();
        }

        public List<Task> GetTasksByEmployeeSubordinates(Employee employee)
        {
            List<Employee> employees = employee.GetSubordinates(x => Equals(x.GetSupervisor(), employee));
            return _tasks.Where(x => x.Comments.Any(modification => employees.Contains(modification.ChangeInitiator))).ToList();
        }

        public List<Task> GetTasksOfWeek(DateTime current)
        {
            DateTime weekAgo = current.AddDays(-7).AddHours(-current.Hour).AddMinutes(-current.Minute).AddSeconds(-current.Second).AddMilliseconds(-current.Millisecond);
            return _tasks.Where(x => DateTime.Compare(x.DateCreated, weekAgo) >= 0).ToList();
        }

        public bool AddTask(Task task)
        {
            if (_tasks.Contains(task))
            {
                throw new ArgumentException("Task already exists in manager", nameof(task));
            }

            _tasks.Add(task);
            return true;
        }

        public bool RemoveTask(Task task)
        {
            return _tasks.Remove(task);
        }

        public bool TryChangeTaskState(Task task, TaskState newState, Employee initiator)
        {
            try
            {
                task.ChangeState(initiator, newState);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool TryReassignToEmployee(Task task, Employee newEmployee, Employee initiator)
        {
            try
            {
                task.AssignToEmployee(initiator, newEmployee);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool TryCommentOnTask(Task task, string comment, Employee initiator)
        {
            try
            {
                task.LeaveComment(initiator, comment);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static bool SameDay(DateTime date1, DateTime date2)
        {
            return date1.Year == date2.Year && date1.Month == date2.Month && date1.Day == date2.Day;
        }
    }
}