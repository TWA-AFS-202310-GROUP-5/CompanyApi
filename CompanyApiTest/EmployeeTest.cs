using CompanyApi;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace CompanyApiTest
{
    public class EmployeeTest
    {
        private HttpClient httpClient;

        public EmployeeTest()
        {
            WebApplicationFactory<Program> webApplicationFactory = new WebApplicationFactory<Program>();
            httpClient = webApplicationFactory.CreateClient();
        }
        [Fact]
        public async Task should_return_created_success_when_create_employee_given_company_id()
        {
            //given
            await ClearDataAsync();
            CreateCompanyRequest companyRequest = new CreateCompanyRequest { Name = "Google" };
            HttpResponseMessage httpResponseMessage1 = await httpClient.PostAsJsonAsync("/api/companies", companyRequest);
            Company company = await httpResponseMessage1.Content.ReadFromJsonAsync<Company>();
            CreateEmployeeRequest employeeRequest =  new CreateEmployeeRequest { Name = "111"};
            // when
            HttpResponseMessage httpResponseMessage2 = await httpClient.PostAsJsonAsync($"api/companies/{company.Id}", employeeRequest);
            //then
            Assert.Equal(HttpStatusCode.Created, httpResponseMessage2.StatusCode);
        }

        [Fact]
        public async Task Should_return_400_bad_request_when_create_employee_given_not_exist_company_id()
        {
            //given
            await ClearDataAsync();
            CreateEmployeeRequest employeeRequest = new CreateEmployeeRequest { Name = "111" };
            string notExistId = Guid.NewGuid().ToString();
            //when
            HttpResponseMessage httpResponseMessage = await httpClient.PostAsJsonAsync($"api/companies/{notExistId}", employeeRequest);
            //then
            Assert.Equal(HttpStatusCode.BadRequest, httpResponseMessage.StatusCode);
        }

        [Fact]
        public async Task Should_return_400_bad_request_when_create_employee_given_existed_employee()
        {
            //given
            await ClearDataAsync();
            CreateCompanyRequest companyRequest = new CreateCompanyRequest { Name = "Google" };
            HttpResponseMessage httpResponseMessage1 = await httpClient.PostAsJsonAsync("/api/companies", companyRequest);
            Company company = await httpResponseMessage1.Content.ReadFromJsonAsync<Company>();
            CreateEmployeeRequest employeeRequest = new CreateEmployeeRequest { Name = "111" };
            await httpClient.PostAsJsonAsync($"api/companies/{company.Id}", employeeRequest);
            //when
            HttpResponseMessage httpResponseMessage = await httpClient.PostAsJsonAsync($"api/companies/{company.Id}", employeeRequest);
            //then
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
            await httpClient.DeleteAsync("/api/companies");
        }
    }
}
