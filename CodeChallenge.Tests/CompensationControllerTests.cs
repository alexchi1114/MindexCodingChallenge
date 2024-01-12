using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;

using CodeChallenge.Models;
using CodeChallenge.Models.Dto;
using CodeCodeChallenge.Tests.Integration.Extensions;
using CodeCodeChallenge.Tests.Integration.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodeCodeChallenge.Tests.Integration
{
    [TestClass]
    public class CompensationControllerTests
    {
        private static HttpClient _httpClient;
        private static TestServer _testServer;

        [ClassInitialize]
        // Attribute ClassInitialize requires this signature
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        public static void InitializeClass(TestContext context)
        {
            _testServer = new TestServer();
            _httpClient = _testServer.NewClient();
        }

        [ClassCleanup]
        public static void CleanUpTest()
        {
            _httpClient.Dispose();
            _testServer.Dispose();
        }

        [TestMethod]
        public void CreateCompensation_Returns_Created()
        {
            // Arrange
            var employeeId = "b7839309-3348-463b-a7e3-5de1c168beb3";
			var compensation = new CompensationForCreationDto()
            {
                Salary = 70000.65M,
                EffectiveDate = new DateTime(2023, 11, 14)
            };

            var requestContent = new JsonSerialization().ToJson(compensation);

            // Execute
            var postRequestTask = _httpClient.PostAsync($"api/employee/{employeeId}/compensation",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            var newCompensation = response.DeserializeContent<CompensationDto>();
            Assert.IsNotNull(newCompensation.CompensationId);
            Assert.AreEqual(compensation.Salary, newCompensation.Salary);
            Assert.AreEqual(compensation.EffectiveDate, newCompensation.EffectiveDate);
            Assert.AreEqual(employeeId, newCompensation.EmployeeId);
        }

		[TestMethod]
		public void GetCompensationById_Returns_Ok()
		{
			// Arrange
			var employeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f";
            var compensationId = "9b675e89-b3b8-4b5a-bdc4-3dea9b8b999b";
            var expectedSalary = 75003.05M;
			var expectedEffectiveDate = new DateTime(2024, 1, 13);

			// Execute
			var getRequestTask = _httpClient.GetAsync($"api/employee/{employeeId}/compensation/{compensationId}");
			var response = getRequestTask.Result;

			// Assert
			Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
			var compensation = response.DeserializeContent<CompensationDto>();
			Assert.AreEqual(employeeId, compensation.EmployeeId);
			Assert.AreEqual(compensationId, compensation.CompensationId);
			Assert.AreEqual(expectedSalary, compensation.Salary);
			Assert.AreEqual(expectedEffectiveDate, compensation.EffectiveDate);
		}

		[TestMethod]
        public void GetCompensationsForEmployee_Returns_Ok()
        {
            // Arrange
            var employeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f";
            var expectedNumberOfCompensations = 2;

            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/employee/{employeeId}/compensation");
            var response = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
			var compensations = response.DeserializeContent<List<CompensationDto>>();
			Assert.AreEqual(expectedNumberOfCompensations, compensations.Count);
		}
	}
}
