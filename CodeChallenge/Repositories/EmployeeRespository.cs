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

		public Employee GetByIdWithReports(string id)
		{
			var employee = _employeeContext.Employees.Include(o => o.DirectReports).SingleOrDefault(e => e.EmployeeId == id);
            if(employee?.DirectReports != null)
            {
                foreach(var report in employee.DirectReports)
                {
                    LoadReports(report);
                }
            }
            return employee;
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

        public Employee Remove(Employee employee)
        {
            return _employeeContext.Remove(employee).Entity;
        }

        private void LoadReports(Employee employee)
        {
            employee = _employeeContext.Employees.Include(o => o.DirectReports).SingleOrDefault(e => e.EmployeeId == employee.EmployeeId);
            //Recursively load the reports
            if(employee?.DirectReports != null)
            {
                foreach(var report in employee.DirectReports)
                {
                    LoadReports(report);
                }
            }
		}
	}
}
