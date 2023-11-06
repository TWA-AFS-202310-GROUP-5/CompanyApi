using CompanyApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;

namespace CompanyApiTest
{
    public class EmployeeApiTest
    {
        private HttpClient httpClient;

        public EmployeeApiTest()
        {
            WebApplicationFactory<Program> webApplicationFactory = new WebApplicationFactory<Program>();
            httpClient = webApplicationFactory.CreateClient();
        }

        [Fact]
        public async Task Should_return_created_employee_with_status_201_when_create_employee_given_a_employee_name()
        {
            // Given
            await ClearDataAsync();
            CreateEmployeeRequest employeeGiven = new CreateEmployeeRequest{Name = "Zhang san"};

            // When
            HttpResponseMessage httpResponseMessage = await httpClient.PostAsync(
                "/api/employees/testCompanyId",
                SerializeObjectToContent(employeeGiven)
            );

            // Then
            Assert.Equal(HttpStatusCode.Created, httpResponseMessage.StatusCode);
            Employee? companyCreated = await DeserializeTo<Employee>(httpResponseMessage);
            Assert.NotNull(companyCreated);
            Assert.NotNull(companyCreated.Id);
            Assert.Equal(employeeGiven.Name, companyCreated.Name);
        }

        [Fact]
        public async Task Should_return_bad_request_when_create_employee_given_a_existed_employee_name()
        {
            // Given
            await ClearDataAsync();
            CreateEmployeeRequest employeeGiven = new CreateEmployeeRequest { Name = "Zhang san" };
            // When
            await httpClient.PostAsync("/api/employees/testCompanyId", SerializeObjectToContent(employeeGiven));
            HttpResponseMessage httpResponseMessage = await httpClient.PostAsync(
                "/api/employees/testCompanyId",
                SerializeObjectToContent(employeeGiven)
            );
            // Then
            Assert.Equal(HttpStatusCode.BadRequest, httpResponseMessage.StatusCode);
        }

        [Fact]
        public async Task Should_return_bad_request_when_create_employee_given_a_employee_with_unknown_field()
        {
            // Given
            await ClearDataAsync();
            StringContent content = new StringContent("{\"unknownField\": \"BlueSky Digital Media\"}", Encoding.UTF8, "application/json");

            // When
            HttpResponseMessage httpResponseMessage = await httpClient.PostAsync("/api/employees/testCompanyId", content);

            // Then
            Assert.Equal(HttpStatusCode.BadRequest, httpResponseMessage.StatusCode);
        }

        private async Task<T?> DeserializeTo<T>(HttpResponseMessage httpResponseMessage)
        {
            string response = await httpResponseMessage.Content.ReadAsStringAsync();
            T? deserializedObject = JsonConvert.DeserializeObject<T>(response);
            return deserializedObject;
        }

        private static StringContent SerializeObjectToContent<T>(T objectGiven)
        {
            return new StringContent(JsonConvert.SerializeObject(objectGiven), Encoding.UTF8, "application/json");
        }

        private async Task ClearDataAsync()
        {
            await httpClient.DeleteAsync("/api/employees");
        }
    }
}
