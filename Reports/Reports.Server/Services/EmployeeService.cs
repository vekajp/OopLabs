using System;
using System.Collections.Generic;
using System.Linq;
using Reports.DAL.Entities.Employees;
namespace Reports.Server.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly TeamLead _teamLead;
        public EmployeeService(TeamLead teamLead)
        {
            _teamLead = teamLead;
        }

        public Employee GetTeamLead()
        {
            return _teamLead;
        }

        public List<Employee> GetAllEmployees()
        {
            return GetEmployees(x => true);
        }

        public List<Employee> GetEmployees(Predicate<Employee> condition)
        {
            return _teamLead.GetSubordinates(condition);
        }

        public Employee FindEmployeeById(Guid id)
        {
            return _teamLead.GetEmployeeById(id);
        }

        public Employee FindEmployeeByName(string name)
        {
            return _teamLead.GetSubordinates(x => x.Name == name).FirstOrDefault();
        }

        public bool TryRemoveEmployee(Employee employee)
        {
            try
            {
                return RemoveEmployee(employee);
            }
            catch
            {
                return false;
            }
        }

        public bool TryAddEmployee(Employee subordinate, Employee supervisor)
        {
            try
            {
                AddEmployee(subordinate, supervisor);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool TryAddEmployee(Employee subordinate)
        {
            try
            {
                AddEmployee(subordinate, _teamLead);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool TeamHasEmployee(Employee employee)
        {
            return FindEmployeeById(employee.Id) != null;
        }

        public bool TryAssignSupervisor(Employee subordinate, Employee supervisor)
        {
            try
            {
                return AssignSupervisor(subordinate, supervisor);
            }
            catch
            {
                return false;
            }
        }

        private bool AssignSupervisor(Employee subordinate, Employee supervisor)
        {
            if (!TeamHasEmployee(supervisor))
            {
                throw new ArgumentException("Specified supervisor is not in a team", nameof(supervisor));
            }

            if (!TeamHasEmployee(subordinate))
            {
                throw new ArgumentException("Specified subordinate is not in a team", nameof(subordinate));
            }

            bool success = supervisor.AddSubordinate(subordinate);
            if (success)
            {
                subordinate.ReassignEmployee(supervisor);
            }

            return success;
        }

        private static bool RemoveEmployee(Employee employee)
        {
            if (employee.GetType() == typeof(TeamLead))
            {
                throw new ArgumentException("Cannot remove team lead", nameof(employee));
            }

            return employee.GetSupervisor().RemoveSubordinate(employee);
        }

        private bool AddEmployee(Employee subordinate, Employee supervisor)
        {
            if (subordinate.GetType() == typeof(TeamLead))
            {
                throw new ArgumentException("Cannot add second team lead", nameof(subordinate));
            }

            if (!TeamHasEmployee(supervisor))
            {
                throw new ArgumentException("Specified supervisor is not in a team", nameof(supervisor));
            }

            if (TeamHasEmployee(subordinate))
            {
                throw new ArgumentException("Specified subordinate is already in a team", nameof(subordinate));
            }

            bool success = supervisor.AddSubordinate(subordinate);
            if (success)
            {
                subordinate.ReassignEmployee(supervisor);
            }

            return success;
        }
    }
}