using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Reports.DAL.Entities.Employees;
using Reports.DAL.EntitiesCreation;
using Reports.Server.DataBase.EntitiesDto;
using Reports.Server.Services;

namespace Reports.Server.Controllers
{
    [ApiController]
    [Route("/employees")]
    public class EmployeeServiceControllers : ControllerBase
    {
        private readonly IReportService _service;
        public EmployeeServiceControllers(IReportService service)
        {
            _service = service;
        }

        [HttpPost("Add new employee to the team")]
        public bool Create([FromQuery] string name, [FromQuery]EmployeeType type, [FromQuery] string supervisorName)
        {
            Employee newEmployee = EmployeeCreator.CreateEmployee(type, name);
            if (string.IsNullOrWhiteSpace(supervisorName)) return _service.TryAddEmployee(newEmployee);
            Employee supervisor = _service.FindEmployeeByName(supervisorName);
            return supervisor != null ? _service.TryAddEmployee(newEmployee, supervisor) : _service.TryAddEmployee(newEmployee);
        }

        [HttpPost("Reassign existing employee to another supervisor")]
        public IActionResult ReassignEmployee([FromQuery] string name, [FromQuery] Guid id, [FromQuery] string supervisorName, [FromQuery] Guid supervisorId)
        {
            Employee employee = GetEmployee(name, id);
            if (employee == null) return NotFound("employee");
            Employee supervisor = GetEmployee(supervisorName, supervisorId);
            if (supervisor == null) return NotFound("supervisor");
            if (_service.TryAssignSupervisor(employee, supervisor))
            {
                return Ok(employee);
            }

            return StatusCode((int)HttpStatusCode.BadRequest);
        }

        [HttpGet("Find employee by name or id")]
        public IActionResult FindEmployee([FromQuery] string name, [FromQuery] Guid id)
        {
            if (string.IsNullOrEmpty(name) && id == Guid.Empty) return StatusCode((int)HttpStatusCode.BadRequest);
            Employee result = GetEmployee(name, id);
            if (result != null)
            {
                return Ok(result);
            }

            return NotFound();
        }

        [HttpGet("Get employees (all/subordinates)")]
        public IActionResult GetEmployees([FromQuery] string supervisorName, [FromQuery] Guid supervisorId)
        {
            Employee supervisor = GetEmployee(supervisorName, supervisorId);
            return Ok(supervisor == null ? _service.GetAllEmployees() : _service.GetEmployees(x => Equals(x.GetSupervisor(), supervisor)));
        }

        [HttpDelete("Remove employee from the team")]
        public IActionResult Delete([FromQuery] string name, [FromQuery] Guid id)
        {
            Employee result = null;
            if (!string.IsNullOrWhiteSpace(name))
            {
                result = _service.FindEmployeeByName(name);
            }

            if (id != Guid.Empty)
            {
                result = _service.FindEmployeeById(id);
            }

            if (result == null) return StatusCode((int)HttpStatusCode.BadRequest);
            bool response = _service.TryRemoveEmployee(result);
            return response ? Ok(true) : Ok("failed");
        }

        private Employee GetEmployee(string name, Guid id)
        {
            Employee result = null;
            if (!string.IsNullOrWhiteSpace(name))
            {
                result = _service.FindEmployeeByName(name);
            }

            if (id != Guid.Empty)
            {
                result = _service.FindEmployeeById(id);
            }

            return result;
        }
    }
}