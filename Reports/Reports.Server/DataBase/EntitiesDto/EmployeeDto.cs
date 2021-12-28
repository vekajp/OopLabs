using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Reports.DAL.Entities.Employees;

namespace Reports.Server.DataBase.EntitiesDto
{
    public sealed class EmployeeDto
    {
        public EmployeeDto()
        {
        }

        public EmployeeDto(Employee employee)
        {
            Id = employee.Id;
            SubordinatesGuids = employee.GetSubordinates(x => x.GetSupervisor() == employee).Select(sub => sub.Id).ToList();
            SupervisorId = employee.GetSupervisorId();
            Name = employee.Name;
            Type = GetType(employee);
        }

        public Guid Id { get; set; }
        public List<Guid> SubordinatesGuids { get; set; }
        public Guid SupervisorId { get; set; }
        public string Name { get; set; }
        public EmployeeType Type { get; set; }
        public void Update(Employee employee)
        {
            SubordinatesGuids = employee.GetSubordinates(x => x.GetSupervisor() == employee).Select(sub => sub.Id).ToList();
            SupervisorId = employee.GetSupervisorId();
        }

        public Employee RestoreEmployee()
        {
            switch (Type)
            {
                case EmployeeType.Worker:
                    return new Worker(Name)
                    {
                        Id = Id,
                    };
                case EmployeeType.Supervisor:
                    return new Supervisor(Name)
                    {
                        Id = Id,
                    };
                case EmployeeType.TeamLead:
                    return new TeamLead(Name)
                    {
                        Id = Id,
                    };
                default:
                    throw new ArgumentOutOfRangeException(nameof(Type));
            }
        }

        private EmployeeType GetType(Employee employee)
        {
            if (employee.GetType() == typeof(TeamLead))
            {
                return EmployeeType.TeamLead;
            }
            else if (employee.GetType() == typeof(Supervisor))
            {
                return EmployeeType.Supervisor;
            }

            return EmployeeType.Worker;
        }
    }
}