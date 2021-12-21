using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Reports.DAL.Entities.TaskRelatedEntities;

namespace Reports.DAL.Entities
{
    public class Report
    {
        private List<Task> _tasks;
        private List<Report> _reports;
        public Report()
        {
            _tasks = new List<Task>();
            _reports = new List<Report>();
        }

        public Report(List<Task> tasks, List<Report> reports)
        {
            _tasks = tasks;
            _reports = reports;
        }

        public void AddTasks(List<Task> tasks)
        {
            tasks.ForEach(x => AddTask(x));
        }

        public void AddReports(List<Report> reports)
        {
            reports.ForEach(x => AddReport(x));
        }

        public bool AddTask(Task task)
        {
            if (_tasks.Contains(task)) return false;
            _tasks.Add(task);
            return true;
        }

        public bool AddReport(Report report)
        {
            if (_reports.Contains(report)) return false;
            _reports.Add(report);
            return true;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            _reports.ForEach(x => builder.Append(x));
            foreach (Task task in _tasks)
            {
                builder.Append($"Task {task.Name} of employee {task.PinedEmployee.Name} :\n");
                task.Comments.ForEach(comment => builder.Append($"\t{comment}\n"));
                builder.Append($"Current state : {task.State}\n");
            }

            return builder.ToString();
        }

        public bool Empty()
        {
            return _reports.Count == 0 && _tasks.Count == 0;
        }
    }
}