using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CodeChallenge.Services;
using CodeChallenge.Models;
using CodeChallenge.Models.Dto;

namespace CodeChallenge.Controllers
{
    [ApiController]
    [Route("api/employee/{employeeId}/compensation")]
    public class CompensationController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IEmployeeService _employeeService;

        public CompensationController(ILogger<EmployeeController> logger, IEmployeeService employeeService)
        {
            _logger = logger;
            _employeeService = employeeService;
        }

        [HttpGet("{compensationId}", Name = "GetCompensationById")]
        public IActionResult GetCompensationById(string employeeId, string compensationId)
        {
            _logger.LogDebug($"Received compensation get request for compensation '{compensationId}'");

            if (_employeeService.GetById(employeeId) == null)
                return NotFound();

            var compensation = _employeeService.GetCompensationById(compensationId);

            if (compensation == null)
                return NotFound();

            //Using a DTO because I don't want to return everything from the Compensation entity (specifically Employee)
            return Ok(new CompensationDto()
            {
                CompensationId = compensation.CompensationId,
                EmployeeId = compensation.EmployeeId,
                EffectiveDate = compensation.EffectiveDate,
                Salary = compensation.Salary
            });
        }

        [HttpGet]
        public IActionResult GetCompensationsForEmployee(string employeeId)
        {
            _logger.LogDebug($"Received compensations get request for employee {employeeId}");

            if (_employeeService.GetById(employeeId) == null)
                return NotFound();

            var compensations = _employeeService.GetCompensationsByEmployeeId(employeeId);

            //Using a DTO because I don't want to return everything from the Compensation entity (specifically Employee)
            return Ok(compensations.Select(compensation => new CompensationDto()
            {
                CompensationId = compensation.CompensationId,
                EmployeeId = compensation.EmployeeId,
                EffectiveDate = compensation.EffectiveDate,
                Salary = compensation.Salary
            }));
        }

        //Using a DTO for creation to limit payload to just necessary information
        [HttpPost]
        public IActionResult CreateCompensationForEmployee(string employeeId, CompensationForCreationDto compensation)
        {
            _logger.LogDebug($"Received compensation create request for employee {employeeId}");

            if (_employeeService.GetById(employeeId) == null)
                return NotFound();

            var compensationEntity = _employeeService.CreateCompensation(employeeId, new Compensation()
            {
                EffectiveDate = compensation.EffectiveDate,
                Salary = compensation.Salary
            });

            return CreatedAtRoute("GetCompensationById", new { employeeId = employeeId, compensationId = compensationEntity.CompensationId }, new CompensationDto()
            {
                CompensationId = compensationEntity.CompensationId,
                EmployeeId = compensationEntity.EmployeeId,
                EffectiveDate = compensationEntity.EffectiveDate,
                Salary = compensationEntity.Salary
            });
        }
    }
}
