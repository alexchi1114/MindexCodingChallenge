using CodeChallenge.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CodeChallenge.Repositories
{
    public interface IEmployeeRepository
    {
        Employee GetById(String id);
        Employee Add(Employee employee);
        Employee Remove(Employee employee);
        ReportingStructure GetReportingStructureByEmployeeId(String id);
        Compensation GetCompensationById(String id);
        Compensation AddCompensation(Compensation compensation);
        List<Compensation> GetCompensationsForEmployee(String employeeId);
        Task SaveAsync();
    }
}