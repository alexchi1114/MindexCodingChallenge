using CodeChallenge.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CodeChallenge.Repositories
{
    public interface IEmployeeRepository
    {
        Employee GetById(String id);
		Employee GetByIdWithReports(String id);
		Employee Add(Employee employee);
        Employee Remove(Employee employee);
        Compensation GetCompensationById(String id);
        Compensation AddCompensation(Compensation compensation);
        List<Compensation> GetCompensationsForEmployee(String employeeId);
        Task SaveAsync();
    }
}