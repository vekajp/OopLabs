using System;
using Reports.DAL.Entities.Employees;
using Reports.Server.DataBase.EntitiesDto;

namespace Reports.DAL.EntitiesCreation
{
    public static class EmployeeCreator
    {
        public static Employee CreateEmployee(EmployeeType type, string name)
        {
            return type switch
            {
                EmployeeType.TeamLead => new TeamLead(name),
                EmployeeType.Worker => new Worker(name),
                EmployeeType.Supervisor => new Supervisor(name),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }
    }
}