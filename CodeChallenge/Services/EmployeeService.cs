using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeChallenge.Models;
using Microsoft.Extensions.Logging;
using CodeChallenge.Repositories;

namespace CodeChallenge.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILogger<EmployeeService> _logger;

        public EmployeeService(ILogger<EmployeeService> logger, IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
            _logger = logger;
        }

        public Employee Create(Employee employee)
        {
            if(employee != null)
            {
                _employeeRepository.Add(employee);
                _employeeRepository.SaveAsync().Wait();
            }

            return employee;
        }

        public Employee GetById(string id)
        {
            if(!String.IsNullOrEmpty(id))
            {
                return _employeeRepository.GetById(id);
            }

            return null;
        }

        public Employee Replace(Employee originalEmployee, Employee newEmployee)
        {
            if(originalEmployee != null)
            {
                _employeeRepository.Remove(originalEmployee);
                if (newEmployee != null)
                {
                    // ensure the original has been removed, otherwise EF will complain another entity w/ same id already exists
                    _employeeRepository.SaveAsync().Wait();

                    _employeeRepository.Add(newEmployee);
                    // overwrite the new id with previous employee id
                    newEmployee.EmployeeId = originalEmployee.EmployeeId;
                }
                _employeeRepository.SaveAsync().Wait();
            }

            return newEmployee;
        }

        //I interpreted the task as wanting to return the entire loaded hierarchy
        public ReportingStructure GetReportingStructureByEmployeeId(string id)
        {
            if (!String.IsNullOrEmpty(id))
            {
                var employee = _employeeRepository.GetByIdWithReports(id);
                if (employee != null)
                {
                    return new ReportingStructure()
                    {
                        Employee = employee,
                        NumberOfReports = GetNumberOfReports(employee)
                    };
                }
            }

            return null;
        }

        private int GetNumberOfReports(Employee employee)
        {
            int _reportingCount = 0;

            //Use recursion to find the total reporting count for the employee.
            if (employee?.DirectReports != null)
            {
                foreach (var report in employee.DirectReports)
                {
                    _reportingCount++;
                    _reportingCount += GetNumberOfReports(report);
                }
            }
            return _reportingCount;
        }

        public Compensation GetCompensationById(string id)
        {
            if (!String.IsNullOrEmpty(id))
            {
                return _employeeRepository.GetCompensationById(id);
            }

            return null;
        }

        public Compensation CreateCompensation(string employeeId, Compensation compensation)
        {
            if (!String.IsNullOrEmpty(employeeId) && compensation != null)
            {
                compensation.EmployeeId = employeeId;
                _employeeRepository.AddCompensation(compensation);
                _employeeRepository.SaveAsync().Wait();
            }

            return compensation;
        }

        public List<Compensation> GetCompensationsByEmployeeId(string employeeId)
        {
            if (!String.IsNullOrEmpty(employeeId))
            {
                return _employeeRepository.GetCompensationsForEmployee(employeeId);
            }

            return null;
        }
    }
}
