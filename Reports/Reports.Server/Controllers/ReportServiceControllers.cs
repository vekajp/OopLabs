using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Reports.DAL.Entities.Employees;
using Reports.DAL.Entities.TaskRelatedEntities;
using Reports.Server.Services;

namespace Reports.Server.Controllers
{
    [ApiController]
    [Route("/reports")]
    public class ReportServiceControllers : ControllerBase
    {
        private readonly IReportService _service;

        public ReportServiceControllers(IReportService service)
        {
            _service = service;
        }

        [HttpPost("Create new weekly report")]
        public IActionResult CreateWeeklyReport()
        {
            return Ok(_service.CreateWeeklyReport());
        }

        [HttpPost("Compose report")]
        public IActionResult ComposeReport([FromQuery] Guid id)
        {
            if (!Authorize(id))
            {
                return StatusCode((int)HttpStatusCode.BadRequest);
            }

            return Ok(_service.ComposeWeeklyReport());
        }

        [HttpPost("Send final report")]
        public IActionResult SendFinalReport([FromQuery] Guid id)
        {
            if (!Authorize(id))
            {
                return StatusCode((int)HttpStatusCode.BadRequest);
            }

            return Ok(_service.SendFinalReport());
        }

        [HttpPost("Get draft report")]
        public IActionResult GetDraftReport([FromQuery] Guid id)
        {
            if (!Authorize(id))
            {
                return StatusCode((int)HttpStatusCode.BadRequest);
            }

            return Ok(_service.GetAuthorized().DraftReport);
        }

        [HttpPost("Add task to report")]
        public IActionResult AddTaskToReport([FromQuery] Guid taskId, [FromQuery] Guid employeeId)
        {
            Task task = _service.FindTaskById(taskId);
            if (task == null)
            {
                return StatusCode((int)HttpStatusCode.BadRequest);
            }

            if (!Authorize(employeeId))
            {
                return StatusCode((int)HttpStatusCode.BadRequest);
            }

            return Ok(_service.AddTaskToReport(task));
        }

        [HttpGet("Get tasks of last week")]
        public IActionResult GetTasksOfLastWeek()
        {
            return Ok(_service.GetTasksOfWeek(DateTime.Now));
        }

        [HttpGet("Get subordinates' reports")]
        public IActionResult GetSubordinatesReports([FromQuery] Guid supervisorId)
        {
            if (!Authorize(supervisorId))
            {
                return StatusCode((int)HttpStatusCode.BadRequest);
            }

            return Ok(_service.GetSubordinatesDailyReports());
        }

        [HttpGet("Get subordinates not reported")]
        public IActionResult GetSubordinatesNotReported([FromQuery] Guid supervisorId)
        {
            if (!Authorize(supervisorId))
            {
                return StatusCode((int)HttpStatusCode.BadRequest);
            }

            return Ok(_service.GetEmployeesNotReported());
        }

        private bool Authorize(Guid id)
        {
            Employee employee = _service.FindEmployeeById(id);
            if (employee == null)
            {
                return false;
            }

            _service.Authorize(employee);
            return true;
        }
    }
}