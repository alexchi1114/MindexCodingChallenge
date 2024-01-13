using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeChallenge.Models;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using CodeChallenge.Data;

namespace CodeChallenge.Repositories
{
    public class EmployeeRespository : IEmployeeRepository
    {
        private readonly EmployeeContext _employeeContext;
        private readonly ILogger<IEmployeeRepository> _logger;

        public EmployeeRespository(ILogger<IEmployeeRepository> logger, EmployeeContext employeeContext)
        {
            _employeeContext = employeeContext;
            _logger = logger;
        }

        public Employee Add(Employee employee)
        {
            employee.EmployeeId = Guid.NewGuid().ToString();
            _employeeContext.Employees.Add(employee);
            return employee;
        }

        public Employee GetById(string id)
        {
            return _employeeContext.Employees.SingleOrDefault(e => e.EmployeeId == id);
        }

        public Employee Remove(Employee employee)
        {
            return _employeeContext.Remove(employee).Entity;
        }

        public ReportingStructure GetReportingStructureByEmployeeId(string id)
        {
            var employee = GetById(id);
            int reportingCount = LoadReports(employee);
            return new ReportingStructure()
            {
                Employee = employee,
                NumberOfReports = reportingCount
            };
        }

        public Compensation GetCompensationById(string id)
        {
            return _employeeContext.Compensation.SingleOrDefault(e => e.CompensationId == id);
        }

        public Compensation AddCompensation(Compensation compensation)
        {
            compensation.CompensationId = Guid.NewGuid().ToString();
            _employeeContext.Compensation.Add(compensation);
            return compensation;
        }

        public List<Compensation> GetCompensationsForEmployee(string employeeId)
        {
            return _employeeContext.Compensation.Where(c => c.EmployeeId == employeeId).OrderByDescending(c => c.EffectiveDate).ToList();
        }

        public Task SaveAsync()
        {
            return _employeeContext.SaveChangesAsync();
        }

        /// <summary>
        /// Loads the DirectReports hierarchy for a given employee and returns the DirectReport count
        /// </summary>
        /// <param name="employee">The employee to load the DirectReports hierarchy for</param>
        /// <param name="usedNodes">The employeeIds that have already been loaded in the hierarchy. Used to prevent infinite loop.</param>
        private int LoadReports(Employee employee, List<string> usedEmployeeIds = null)
        {
            //Using recursion to load the reports. Ideally, this would be done with a recursive CTE in, for example, SQL Server.
            usedEmployeeIds ??= new List<string>() { employee.EmployeeId };
            int _reportingCount = 0;
            employee = _employeeContext.Employees.Include(o => o.DirectReports).SingleOrDefault(e => e.EmployeeId == employee.EmployeeId);
            if(employee?.DirectReports != null)
            {
                foreach(var report in employee.DirectReports)
                {
                    if (usedEmployeeIds == null || !usedEmployeeIds.Contains(report.EmployeeId))
                    {
                        //Some defensive code to Keep track of which employeeIds have been used to prevent a branch looping back on itself
                        usedEmployeeIds.Add(report.EmployeeId);
                        _reportingCount++;
                        _reportingCount += LoadReports(report, usedEmployeeIds);
                    }
                    else
                    {
                        throw new StackOverflowException();
                    }
                }
            }
            return _reportingCount;
        }
    }
}