using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Reports.DAL.Entities.Employees;
using Reports.DAL.Entities.TaskRelatedEntities;
using Reports.Server.Services;

namespace Reports.Server.Controllers
{
    [ApiController]
    [Route("/tasks")]
    public class TaskManagerControllers : ControllerBase
    {
        private IReportService _service;
        public TaskManagerControllers(IReportService service)
        {
            _service = service;
        }

        [HttpGet("Get all tasks")]
        public IActionResult GetAllTasks()
        {
            return Ok(_service.GetAllTasks());
        }

        [HttpGet("Get task by id")]
        public IActionResult FindTaskById([FromQuery] Guid id)
        {
            if (id == Guid.Empty) return StatusCode((int)HttpStatusCode.BadRequest);
            Task task = _service.FindTaskById(id);
            if (task == null) return NotFound();
            return Ok(task);
        }

        [HttpGet("Find tasks by date created")]
        public IActionResult FindTasksByDateCreated([FromQuery] DateTime date)
        {
            if (string.IsNullOrEmpty(date.ToString(CultureInfo.InvariantCulture))) return StatusCode((int)HttpStatusCode.BadRequest);
            return Ok(_service.GetTasksByDateCreated(date));
        }

        [HttpGet("Find tasks by date modified")]
        public IActionResult FindTasksByDateLastModified([FromQuery] DateTime date)
        {
            if (string.IsNullOrEmpty(date.ToString(CultureInfo.InvariantCulture))) return StatusCode((int)HttpStatusCode.BadRequest);
            return Ok(_service.GetTasksByDateLastModified(date));
        }

        [HttpGet("Get tasks of employee")]
        public IActionResult FindTasksByEmployee([FromQuery] Guid id)
        {
            if (id == Guid.Empty) return StatusCode((int)HttpStatusCode.BadRequest);
            Employee employee = _service.FindEmployeeById(id);
            return employee == null ? NotFound() : Ok(_service.GetTasksByEmployee(employee));
        }

        [HttpGet("Get tasks modified by employee")]
        public IActionResult FindTasksModifiedByEmployee([FromQuery] Guid id)
        {
            if (id == Guid.Empty) return StatusCode((int)HttpStatusCode.BadRequest);
            Employee employee = _service.FindEmployeeById(id);
            return employee == null ? NotFound() : Ok(_service.GetTasksModifiedByEmployee(employee));
        }

        [HttpGet("Get tasks of employee's subordinates")]
        public IActionResult FindTasksOfEmployeeSubordinates([FromQuery] Guid id)
        {
            if (id == Guid.Empty) return StatusCode((int)HttpStatusCode.BadRequest);
            Employee employee = _service.FindEmployeeById(id);
            return employee == null ? NotFound() : Ok(_service.GetTasksByEmployeeSubordinates(employee));
        }

        [HttpPost("Create task")]
        public IActionResult CreateTask([FromQuery] string name)
        {
            Employee authorized = _service.GetAuthorized();
            if (authorized == null)
            {
                return StatusCode((int)HttpStatusCode.BadRequest);
            }

            var task = new Task(name, _service.GetAuthorized());
            return Ok(_service.AddTask(task));
        }

        [HttpPost("Change task state")]
        public IActionResult ChangeTaskState([FromQuery] Guid id, [FromQuery] TaskState newState)
        {
            Task task = _service.FindTaskById(id);
            if (task == null)
            {
                return StatusCode((int)HttpStatusCode.BadRequest);
            }

            return Ok(_service.TryChangeTaskState(task, newState, _service.GetAuthorized()));
        }

        [HttpPost("Reassign task")]
        public IActionResult ReassignTask([FromQuery] Guid id, [FromQuery] Guid employeeId)
        {
            Task task = _service.FindTaskById(id);
            if (task == null)
            {
                return StatusCode((int)HttpStatusCode.BadRequest);
            }

            Employee employee = _service.FindEmployeeById(employeeId);
            if (employee == null)
            {
                return StatusCode((int)HttpStatusCode.BadRequest);
            }

            return Ok(_service.TryReassignToEmployee(task, employee, _service.GetAuthorized()));
        }

        [HttpPost("Comment on task")]
        public IActionResult CommentOnTask([FromQuery] Guid id, [FromQuery] string comment)
        {
            Task task = _service.FindTaskById(id);
            if (task == null)
            {
                return StatusCode((int)HttpStatusCode.BadRequest);
            }

            return Ok(_service.TryCommentOnTask(task, comment, _service.GetAuthorized()));
        }
    }
}