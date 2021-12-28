using System;
using System.Collections.Generic;
using Reports.DAL.Entities;
using Reports.DAL.Entities.Employees;
using Reports.Server.DataBase.EntitiesDto;

namespace Reports.Server.Services
{
    public interface IEmployeeService
    {
        Employee GetTeamLead();
        List<Employee> GetAllEmployees();
        List<Employee> GetEmployees(Predicate<Employee> condition);
        Employee FindEmployeeById(Guid id);
        Employee FindEmployeeByName(string name);
        bool TryRemoveEmployee(Employee employee);
        bool TryAddEmployee(Employee subordinate, Employee supervisor);
        bool TryAddEmployee(Employee subordinate);
        bool TeamHasEmployee(Employee employee);
        bool TryAssignSupervisor(Employee subordinate, Employee supervisor);
    }
}