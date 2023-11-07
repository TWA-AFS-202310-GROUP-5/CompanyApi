using CompanyApi;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Json;
using System.Text;

namespace CompanyApiTest
{
    public class CompanyApiTest
    {
        private HttpClient httpClient;

        public CompanyApiTest()
        {
            WebApplicationFactory<Program> webApplicationFactory = new WebApplicationFactory<Program>();
            httpClient = webApplicationFactory.CreateClient();
        }

        [Fact]
        public async Task Should_return_created_company_with_status_201_when_create_cpmoany_given_a_company_name()
        {
            // Given
            await ClearDataAsync();
            CreateCompanyRequest companyGiven = new CreateCompanyRequest{Name = "BlueSky Digital Media" };

        // When
            HttpResponseMessage httpResponseMessage = await httpClient.PostAsync(
                "/api/companies", 
                SerializeObjectToContent(companyGiven)
            );
           
            // Then
            Assert.Equal(HttpStatusCode.Created, httpResponseMessage.StatusCode);
            Company? companyCreated = await DeserializeTo<Company>(httpResponseMessage);
            Assert.NotNull(companyCreated);
            Assert.NotNull(companyCreated.Id);
            Assert.Equal(companyGiven.Name, companyCreated.Name);
        }

        [Fact]
        public async Task Should_return_bad_reqeust_when_create_company_given_a_existed_company_name()
        {
            // Given
            await ClearDataAsync();
            Company companyGiven = new Company("BlueSky Digital Media");

            // When
            await httpClient.PostAsync("/api/companies", SerializeObjectToContent(companyGiven));
            HttpResponseMessage httpResponseMessage = await httpClient.PostAsync(
                "/api/companies", 
                SerializeObjectToContent(companyGiven)
            );
            // Then
            Assert.Equal(HttpStatusCode.BadRequest, httpResponseMessage.StatusCode);
        }

        [Fact]
        public async Task Should_return_bad_reqeust_when_create_company_given_a_company_with_unknown_field()
        {
            // Given
            await ClearDataAsync();
            StringContent content = new StringContent("{\"unknownField\": \"BlueSky Digital Media\"}", Encoding.UTF8, "application/json");
          
            // When
            HttpResponseMessage httpResponseMessage = await httpClient.PostAsync("/api/companies", content);
           
            // Then
            Assert.Equal(HttpStatusCode.BadRequest, httpResponseMessage.StatusCode);
        }

        [Fact]
        public async Task Should_return_all_companys_when_get_companies_given_nothing()
        {
            // Given
            ClearDataAsync();
            var companyGiven1 = new CreateCompanyRequest { Name = "BlueSky Digital Media 1" };
            var companyGiven2 = new CreateCompanyRequest { Name = "BlueSky Digital Media 2" };


            // When
            HttpResponseMessage httpResponseMessage1 = await httpClient.PostAsJsonAsync("/api/companies", companyGiven1);
            HttpResponseMessage httpResponseMessage2 = await httpClient.PostAsJsonAsync("/api/companies", companyGiven2);

            var companies = await httpClient.GetFromJsonAsync<List<Company>>("api/companies");
            // Then

            Assert.Equal(2, companies.Count);
        }


        [Fact]
        public async Task Should_return_a_company_when_get_company_given_company_id()
        {
            // Given
            ClearDataAsync();
            var companyGiven1 = new CreateCompanyRequest { Name = "BlueSky Digital Media 1" };

            // When
            HttpResponseMessage createdResponseMessage = await httpClient.PostAsJsonAsync("/api/companies", companyGiven1);
            var createdCompany = await DeserializeTo<Company>(createdResponseMessage);
            HttpResponseMessage getResponseMessage = await httpClient.GetAsync($"/api/companies/{createdCompany.Id}");

            var company = await DeserializeTo<Company>(getResponseMessage);

            // Then
            Assert.Equal(HttpStatusCode.OK, getResponseMessage.StatusCode);
            Assert.Equal(createdCompany.Id, company.Id);

        }

        [Fact]
        public async Task Should_return_404__when_get_company_given_do_not_exit_company_id()
        {
            ClearDataAsync();
            // Given

            // When
            HttpResponseMessage getResponseMessage = await httpClient.GetAsync("/api/companies/BlueSky Digital Media 1");

            var company = await DeserializeTo<Company>(getResponseMessage);
            
            // Then
            Assert.Equal(HttpStatusCode.NotFound, getResponseMessage.StatusCode);

        }

        [Fact]
        public async Task Should_return_success_when_update_company_given_company_id()
        {
            // Given
            ClearDataAsync();
            var companyGiven1 = new CreateCompanyRequest { Name = "BlueSky Digital Media 1" };

            // When
            HttpResponseMessage createdResponseMessage = await httpClient.PostAsJsonAsync("/api/companies", companyGiven1);
            var createdCompany = await DeserializeTo<Company>(createdResponseMessage);

            var updateCompanyRequest = new UpdateCompanyRequest { Name = "New name" };
            var updateString = SerializeObjectToContent(updateCompanyRequest);
            HttpResponseMessage getResponseMessage = await httpClient.PutAsync($"/api/companies/{createdCompany.Id}",updateString);
            
            // Then
            Assert.Equal(HttpStatusCode.NoContent, getResponseMessage.StatusCode);

        }

        [Fact]
        public async Task Should_return_not_found_when_update_company_given_do_not_exit_company_id()
        {
            // Given
            ClearDataAsync();

            // When

            var updateCompanyRequest = new UpdateCompanyRequest { Name = "New name" };
            var updateString = SerializeObjectToContent(updateCompanyRequest);
            HttpResponseMessage getResponseMessage = await httpClient.PutAsync("/api/companies/NotExitId", updateString);

            // Then
            Assert.Equal(HttpStatusCode.NotFound, getResponseMessage.StatusCode);

        }

        [Fact]
        public async Task Should_return_ok_when_get_page_given_page_and_size()
        {
            ClearDataAsync();
            var uCompanyRequests = new List<CreateCompanyRequest>();
            for (int i = 0; i < 30; i++)
            {
                uCompanyRequests.Add(new CreateCompanyRequest{Name = $"company {i}"});
            }

            for (int i = 0; i < 10; i++)
            {
                 await httpClient.PostAsJsonAsync("/api/companies", uCompanyRequests[i]);

            }

            var companies = await httpClient.GetFromJsonAsync<List<Company>>("api/companies/2/3");
            Assert.Equal(2,companies.Count);
            Assert.Equal(uCompanyRequests[4].Name, companies[0].Name);
            Assert.Equal(uCompanyRequests[5].Name, companies[1].Name);
            await ClearDataAsync();


        }


        [Fact]
        public async Task Should_return_employee_when_created_given_employee()
        {
            // Given
            await ClearDataAsync();
            CreateCompanyRequest companyGiven = new CreateCompanyRequest { Name = "BlueSky Digital Media" };

            // When
            HttpResponseMessage httpResponseMessage = await httpClient.PostAsync(
                "/api/companies",
                SerializeObjectToContent(companyGiven)
            );

            var createdEmployee = new CreateEmployeeRequest { Name = "Zhang san" };
            var company = await DeserializeTo<Company>(httpResponseMessage);
            HttpResponseMessage httpCreatedEmpResponseMessage = await httpClient.PostAsync(
                $"/api/companies/{company.Id}/",
                SerializeObjectToContent(createdEmployee)
            );
            var created = await DeserializeTo<Employee>(httpCreatedEmpResponseMessage);


            // Then
            Assert.Equal(HttpStatusCode.Created, httpCreatedEmpResponseMessage.StatusCode);
            Assert.NotNull(created);
            Assert.NotNull(created.Id);
            Assert.Equal("Zhang san", created.Name);


        }

        [Fact]
        public async Task Should_return_bad_reques_when_created_employee_given_company_does_not_exit()
        {

            var createdEmployee = new CreateEmployeeRequest { Name = "Zhang san" };

            HttpResponseMessage httpCreatedEmpResponseMessage = await httpClient.PostAsync(
                "/api/companies/companyId/",
                SerializeObjectToContent(createdEmployee)
            );
            var created = await DeserializeTo<Employee>(httpCreatedEmpResponseMessage);

 
            Assert.Equal(HttpStatusCode.BadRequest, httpCreatedEmpResponseMessage.StatusCode);


        }

        [Fact]
        public async Task Should_return_Ok_when_delete_employee_given_companyId_and_employeeId()
        {

            await ClearDataAsync();
            CreateCompanyRequest companyGiven = new CreateCompanyRequest { Name = "BlueSky Digital Media" };

            HttpResponseMessage httpResponseMessage = await httpClient.PostAsync(
                "/api/companies",
                SerializeObjectToContent(companyGiven)
            );

            var createdEmployee = new CreateEmployeeRequest { Name = "Zhang san" };
            var company = await DeserializeTo<Company>(httpResponseMessage);
            HttpResponseMessage httpCreatedEmpResponseMessage = await httpClient.PostAsync(
                $"/api/companies/{company.Id}/",
                SerializeObjectToContent(createdEmployee)
            );
            var created = await DeserializeTo<Employee>(httpCreatedEmpResponseMessage);

            var deleteInfo = await httpClient.DeleteAsync($"/api/companies/{company.Id}/{created.Id}");

            Assert.Equal(HttpStatusCode.OK, deleteInfo.StatusCode);
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