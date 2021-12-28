using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Text.Json.Serialization;
using Reports.DAL.Entities.Employees;
using Reports.DAL.Entities.TaskRelatedEntities;

namespace Reports.DAL.Entities
{
    public class Report
    {
        public Report(Employee author)
        {
            Tasks = new List<Task>();
            Reports = new List<Report>();
            Author = author;
            Id = Guid.NewGuid();
        }

        public Report(Report report)
        {
            Tasks = new List<Task>();
            Reports = new List<Report>();
            report.Reports.ForEach(x => Reports.Add(x));
            report.Tasks.ForEach(x => Tasks.Add(x));
            Author = report.Author;
            Id = report.Id;
        }

        public Report(List<Task> tasks, List<Report> reports)
        {
            Tasks = tasks;
            Reports = reports;
        }

        public Guid Id { get; init; }
        public Employee Author { get; private set; }
        public List<Task> Tasks { get; set; }
        public List<Report> Reports { get; set; }
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
            if (Tasks.Contains(task)) return false;
            Tasks.Add(task);
            return true;
        }

        public bool AddReport(Report report)
        {
            if (Reports.Contains(report)) return false;
            Reports.Add(report);
            return true;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            Reports.ForEach(x => builder.Append(x));
            foreach (Task task in Tasks)
            {
                builder.Append($"Task {task.Name} of employee {task.PinedEmployee.Name} :\n");
                task.Comments.ForEach(comment => builder.Append($"\t{comment}\n"));
                builder.Append($"Current state : {task.State}\n");
            }

            return builder.ToString();
        }

        public bool Empty()
        {
            return Reports.Count == 0 && Tasks.Count == 0;
        }

        public void Clear()
        {
            Tasks = new List<Task>();
            Reports = new List<Report>();
        }
    }
}