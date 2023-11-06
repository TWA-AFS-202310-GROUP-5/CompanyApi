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
        public async Task Should_return_created_company_with_status_201_when_create_company_given_a_company_name()
        {
            // Given
            await ClearDataAsync();
            CreateCompanyRequest companyGiven = new CreateCompanyRequest("BlueSky Digital Media");

            // When
            HttpResponseMessage httpResponseMessage = await httpClient.PostAsJsonAsync("/api/companies", companyGiven);
             
            // Then
            Assert.Equal(HttpStatusCode.Created, httpResponseMessage.StatusCode);
            Company? companyCreated = await httpResponseMessage.Content.ReadFromJsonAsync<Company>();
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
        public async Task Should_return_all_companies_when_GetAll_given_companies_created()
        {
            //given
            await ClearDataAsync();
            CreateCompanyRequest companyGiven = new CreateCompanyRequest("BlueSky Digital Media");

            await httpClient.PostAsJsonAsync("api/companies", companyGiven);
            //when
            HttpResponseMessage httpResponseMessage = await httpClient.GetAsync("api/companies");

            var response = await httpResponseMessage.Content.ReadFromJsonAsync<List<Company>>();
            //then
            Assert.Equal(HttpStatusCode.OK, httpResponseMessage.StatusCode);
            Assert.Equal(companyGiven.Name, response[0].Name);
        }

        [Fact]
        public async Task Should_return_request_company_when_Get_given_company_Id()
        {
            //given
            await httpClient.DeleteAsync("/api/companies");
            Company companyGiven1 = new Company("BlueSky Digital Media");
            HttpResponseMessage httpResponseMessage1  =  await httpClient.PostAsJsonAsync("api/companies", companyGiven1);
            Company companyGiven2 = new Company("Google");
            await httpClient.PostAsJsonAsync("api/companies", companyGiven2);
           
            var company1 = await httpResponseMessage1.Content.ReadFromJsonAsync<Company>();

            //when
            HttpResponseMessage httpResponseMessage = await httpClient.GetAsync($"api/companies/{company1?.Id}");

            Company? companyGet = await DeserializeTo<Company>(httpResponseMessage);
            Assert.Equal(HttpStatusCode.OK, httpResponseMessage.StatusCode);
            Assert.Equal(company1.Name, companyGet.Name);
            Assert.Equal(company1.Id, companyGet.Id);
        }

        [Fact]
        public async Task Should_return_404_when_Get_given_company_Id_not_existed()
        {
            //given
            string id1 = "12345";

            //when
            HttpResponseMessage httpResponseMessage = await httpClient.GetAsync($"api/companies/{id1}");

            Company? companyGet = await httpResponseMessage.Content.ReadFromJsonAsync<Company>();
            Assert.Equal(HttpStatusCode.NotFound, httpResponseMessage.StatusCode);
        }
        

        [Fact]
        public async Task Should_return_pagesize_companies_from_pageindex_when_get_given_pageSize_and_pageIndex()
        {
            //given
            await ClearDataAsync();
            for (int i = 0; i < 10; i++)
            {
                CreateCompanyRequest companyGiven = new CreateCompanyRequest($"BlueSky Digital Media{i}");
                await httpClient.PostAsJsonAsync("api/companies", companyGiven);
            }

            //when
            HttpResponseMessage httpResponseMessage = await httpClient.GetAsync($"api/companies?pageSize=2&pageIndex=3");

            List<Company>? companies = await httpResponseMessage.Content.ReadFromJsonAsync<List<Company>>();

            //then
            Assert.Equal("BlueSky Digital Media4", companies[0].Name);
            Assert.Equal(2, companies.Count);

        }

        [Fact]
        public async Task Should_return_204_when_update_successfully_given_id_and_updatedCompany()
        {
            //given
            await ClearDataAsync();
            CreateCompanyRequest companyGiven = new CreateCompanyRequest("BlueSky Digital Media");
            CreateCompanyRequest newCompany = new CreateCompanyRequest("BlueSky");
            HttpResponseMessage httpResponseMessage = await httpClient.PostAsJsonAsync("api/companies", companyGiven);
            Company? company = await httpResponseMessage.Content.ReadFromJsonAsync<Company>();

            HttpResponseMessage httpResponseMessage2 = await httpClient.PutAsJsonAsync($"api/companies/{company.Id}", newCompany);

            Assert.Equal(HttpStatusCode.NoContent, httpResponseMessage2.StatusCode);

        }

        [Fact]
        public async Task Should_return_404_when_update_given_not_exist_id_and_updatedCompany()
        {
            await ClearDataAsync();
            CreateCompanyRequest companyGiven = new CreateCompanyRequest("BlueSky Digital Media");

            HttpResponseMessage httpResponseMessage = await httpClient.PutAsJsonAsync($"api/companies/0000", companyGiven);

            Assert.Equal(HttpStatusCode.NotFound, httpResponseMessage.StatusCode);

        }

        [Fact]
        public async Task Should_return_employee_with_201_when_add_employee_to_the_company_given_companyId_and_Employee()
        {
            await ClearDataAsync();

            CreateCompanyRequest companyGiven = new CreateCompanyRequest("BlueSky Digital Media");
            HttpResponseMessage httpResponseMessage = await httpClient.PostAsJsonAsync("api/companies", companyGiven);
            Company? company = await httpResponseMessage.Content.ReadFromJsonAsync<Company>();
            var employee = new EmployeeRequest(10000, "AA");

            HttpResponseMessage httpResponseMessage2 = await httpClient.PostAsJsonAsync($"api/companies/{company.Id}/employees", employee);

            Employee? employee2 = await httpResponseMessage2.Content.ReadFromJsonAsync<Employee>();
            var newCompany = await (await httpClient.GetAsync($"api/companies/{company.Id}")).Content.ReadFromJsonAsync<Company>();

            //then
            Assert.Equal(HttpStatusCode.Created, httpResponseMessage2.StatusCode);
            Assert.Equal(employee.Name, employee2.Name);
            Assert.Equal(employee.Salary, employee2.Salary);
            Assert.Equal(newCompany.Employees[0].Id, employee2.Id);
            Assert.NotNull(employee2.Id);
        }


        [Fact]
        public async Task Should_return_404_when_add_employee_to_company_given_not_exist_companyId_and_Employee()
        {
            //given
            await ClearDataAsync();

            var employee = new EmployeeRequest(10000, "Kitty");

            //when
            HttpResponseMessage httpResponseMessage = await httpClient.PostAsJsonAsync($"api/companies/111/employees", employee);

            //then
            Assert.Equal(HttpStatusCode.NotFound, httpResponseMessage.StatusCode);

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