using System;
using System.Linq;

namespace CodeChallenge.Models.Dto
{
    public class CompensationDto
    {
        public string CompensationId { get; set; }
        public string EmployeeId { get; set; }
        public decimal Salary { get; set; }
        public DateTime EffectiveDate { get; set; }
    }
}
