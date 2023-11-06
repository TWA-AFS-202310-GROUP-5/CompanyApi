using CompanyApi;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Json;
using System.Text;

namespace CompanyApiTest
{
    public class CompanyApiTest : IDisposable
    {
        private HttpClient httpClient;

        public CompanyApiTest()
        {
            WebApplicationFactory<Program> webApplicationFactory = new WebApplicationFactory<Program>();
            httpClient = webApplicationFactory.CreateClient();
        }

        public async void Dispose()
        {
            await httpClient.DeleteAsync("/api/companies");
        }

        [Fact]
        public async Task Should_return_created_company_with_status_201_when_create_comoany_given_a_company_name()
        {
            // Given
            CreateCompanyRequest companyGiven = new CreateCompanyRequest { Name = "BlueSky Digital Media" };

            // When
            HttpResponseMessage httpResponseMessage = await httpClient.PostAsJsonAsync("/api/companies", 
                companyGiven);
            Company? companyCreated = await httpResponseMessage.Content.ReadFromJsonAsync<Company>();
           
            // Then
            Assert.Equal(HttpStatusCode.Created, httpResponseMessage.StatusCode);
            Assert.NotNull(companyCreated);
            Assert.NotNull(companyCreated.Id);
            Assert.Equal(companyGiven.Name, companyCreated.Name);
        }

        [Fact]
        public async Task Should_return_bad_reqeust_when_create_company_given_a_existed_company_name()
        {
            // Given
            CreateCompanyRequest companyGiven = new CreateCompanyRequest {Name = "BlueSky Digital Media" };
            await httpClient.PostAsJsonAsync("/api/companies", companyGiven);

            // When
            HttpResponseMessage httpResponseMessage = await httpClient.PostAsJsonAsync("/api/companies",
    companyGiven);
            Company? companyCreated = await httpResponseMessage.Content.ReadFromJsonAsync<Company>();

            // Then
            Assert.Null(companyCreated?.Name);
            Assert.Equal(HttpStatusCode.BadRequest, httpResponseMessage.StatusCode);
        }

        [Fact]
        public async Task Should_return_bad_reqeust_when_create_company_given_a_company_with_unknown_field()
        {
            // Given
            StringContent content = new StringContent("{\"unknownField\": \"BlueSky Digital Media\"}", Encoding.UTF8, "application/json");
          
            // When
            HttpResponseMessage httpResponseMessage = await httpClient.PostAsync("/api/companies", content);
            Company? companyCreated = await httpResponseMessage.Content.ReadFromJsonAsync<Company>();

            // Then
            Assert.Null(companyCreated?.Name);
            Assert.Equal(HttpStatusCode.BadRequest, httpResponseMessage.StatusCode);
        }

        [Fact]
        public async Task Should_return_companies_with_status_ok_when_get_given_exists_companies()
        {
            //Given
            CreateCompanyRequest companyGiven1 = new CreateCompanyRequest { Name = "BlueSky Digital Media" };
            CreateCompanyRequest companyGiven2 = new CreateCompanyRequest { Name = "Google" };
            await httpClient.PostAsJsonAsync("/api/companies", companyGiven1);
            await httpClient.PostAsJsonAsync("/api/companies", companyGiven2);

            //When
            HttpResponseMessage httpResponseMessage = await httpClient.GetAsync("/api/companies");
            List<Company>? result = await httpResponseMessage.Content.ReadFromJsonAsync<List<Company>>();

            //Then
            Assert.Equal(2, result?.Count);
            Assert.Equal(companyGiven1.Name, result?[0].Name);
            Assert.Equal(companyGiven2.Name, result?[1].Name);
            Assert.Equal(HttpStatusCode.OK, httpResponseMessage.StatusCode);
        }

        [Fact]
        public async Task Should_return_nothing_with_status_ok_when_get_given_no_exist_companies()
        {
            //Given

            //When
            HttpResponseMessage httpResponseMessage = await httpClient.GetAsync("/api/companies");
            List<Company>? result = await httpResponseMessage.Content.ReadFromJsonAsync<List<Company>>();

            //Then
            Assert.Equal(0, result?.Count);
            Assert.Equal(HttpStatusCode.OK, httpResponseMessage.StatusCode);
        }

        [Fact]
        public async Task Should_return_company_found_with_status_ok_when_get_given_found_the_company_with_id()
        {
            //Given
            CreateCompanyRequest companyGiven = new CreateCompanyRequest { Name = "BlueSky Digital Media" };
            var PostResult = await httpClient.PostAsJsonAsync("api/companies", companyGiven);
            Company? companyReturnedFromPost = await PostResult.Content.ReadFromJsonAsync<Company>();

            //When
            HttpResponseMessage httpResponseMessage = await httpClient.GetAsync($"api/companies/{companyReturnedFromPost?.Id}");
            Company? companyReturnedFromGet = await httpResponseMessage.Content.ReadFromJsonAsync<Company>();

            //Then
            Assert.Equal(companyReturnedFromPost, companyReturnedFromGet);
            Assert.Equal(HttpStatusCode.OK, httpResponseMessage.StatusCode);
        }

        [Fact]
        public async Task Should_return_status_not_found_when_get_given_not_found_the_company_with_id()
        {
            //Given

            //When
            HttpResponseMessage message = await httpClient.GetAsync($"api/companies/{new Guid()}");
            Company? companyReturnedFromGet = await message.Content.ReadFromJsonAsync<Company>();

            //Then
            Assert.Null(companyReturnedFromGet?.Name);
            Assert.Equal(HttpStatusCode.NotFound, message.StatusCode);
        }

        [Fact]
        public async Task Should_return_companies_in_given_page_with_status_ok_when_get_given_exists_companies_and_pageSize_and_pageIndex()
        {
            //Given
            CreateCompanyRequest companyGiven1 = new CreateCompanyRequest { Name = "BlueSky Digital Media" };
            CreateCompanyRequest companyGiven2 = new CreateCompanyRequest { Name = "Google" };
            CreateCompanyRequest companyGiven3 = new CreateCompanyRequest { Name = "Amazon" };
            CreateCompanyRequest companyGiven4 = new CreateCompanyRequest { Name = "Apple" };
            await httpClient.PostAsJsonAsync("/api/companies", companyGiven1);
            await httpClient.PostAsJsonAsync("/api/companies", companyGiven2);
            await httpClient.PostAsJsonAsync("/api/companies", companyGiven3);
            await httpClient.PostAsJsonAsync("/api/companies", companyGiven4);

            //When
            HttpResponseMessage httpResponseMessage = await httpClient.GetAsync("/api/companies?pageSize=2&pageIndex=2");
            List<Company>? result = await httpResponseMessage.Content.ReadFromJsonAsync<List<Company>>();

            //Then
            Assert.Equal(2, result?.Count);
            Assert.Equal(companyGiven3.Name, result?[0].Name);
            Assert.Equal(companyGiven4.Name, result?[1].Name);
            Assert.Equal(HttpStatusCode.OK, httpResponseMessage.StatusCode);
        }

        [Fact]
        public async Task Should_return_nothing_with_status_ok_when_get_given_not_enought_companies_for_pageSize_and_pageIndex()
        {
            //Given

            //When
            HttpResponseMessage httpResponseMessage = await httpClient.GetAsync("/api/companies?pageSize=2&pageIndex=2");
            List<Company>? result = await httpResponseMessage.Content.ReadFromJsonAsync<List<Company>>();

            //Then
            Assert.Equal(0, result?.Count);
            Assert.Equal(HttpStatusCode.OK, httpResponseMessage.StatusCode);
        }

        [Fact]
        public async Task Should_return_updated_company_with_status_ok_when_put_given_company_info_with_id_can_be_found()
        {
            //Given
            CreateCompanyRequest companyGiven = new CreateCompanyRequest { Name = "BlueSky Digital Media" };
            CreateCompanyRequest companyGivenNew = new CreateCompanyRequest { Name = "Google" };
            var PostResult = await httpClient.PostAsJsonAsync("api/companies", companyGiven);
            Company? companyReturnedFromPost = await PostResult.Content.ReadFromJsonAsync<Company>();

            //When
            HttpResponseMessage httpResponseMessage = await httpClient.PutAsJsonAsync($"api/companies/{companyReturnedFromPost?.Id}", companyGivenNew);
            Company? result = await httpResponseMessage.Content.ReadFromJsonAsync<Company>();

            //Then
            Assert.Equal("Google", result?.Name);
            Assert.Equal(companyReturnedFromPost?.Id, result?.Id);
            Assert.Equal(HttpStatusCode.OK, httpResponseMessage.StatusCode);
        }

        [Fact]
        public async Task Should_return_nothing_with_status_not_found_when_put_given_company_info_with_not_exist_id()
        {
            //Given
            CreateCompanyRequest companyGivenNew = new CreateCompanyRequest { Name = "Google" };

            //When
            HttpResponseMessage message = await httpClient.PutAsJsonAsync($"api/companies/{new Guid()}", companyGivenNew);
            Company? companyReturnedFromGet = await message.Content.ReadFromJsonAsync<Company>();

            //Then
            Assert.Null(companyReturnedFromGet?.Name);
            Assert.Equal(HttpStatusCode.NotFound, message.StatusCode);
        }

        [Fact]
        public async Task Should_return_created_employee_with_status_201_when_create_employee_given_a_company_id_and_non_existed_employeeName()
        {
            // Given
            CreateCompanyRequest companyGiven = new CreateCompanyRequest { Name = "BlueSky Digital Media" };
            HttpResponseMessage httpResponseMessage = await httpClient.PostAsJsonAsync("/api/companies",
                companyGiven);
            Company? companyCreated = await httpResponseMessage.Content.ReadFromJsonAsync<Company>();
            CreateEmployeeRequest employeeGiven = new CreateEmployeeRequest { Name = "John" };

            // When
            httpResponseMessage = await httpClient.PostAsJsonAsync($"/api/companies/{companyCreated?.Id}/employees", employeeGiven);
            Employee? employeeCreated = await httpResponseMessage.Content.ReadFromJsonAsync<Employee>();

            // Then
            Assert.Equal(HttpStatusCode.Created, httpResponseMessage.StatusCode);
            Assert.NotNull(employeeCreated);
            Assert.NotNull(employeeCreated.Id);
            Assert.Equal(employeeGiven.Name, employeeCreated.Name);
        }

        [Fact]
        public async Task Should_return_bad_reqeust_when_create_employee_given_a_company_id_and_existed_employeeName()
        {
            // Given
            CreateCompanyRequest companyGiven = new CreateCompanyRequest { Name = "BlueSky Digital Media" };
            HttpResponseMessage httpResponseMessage = await httpClient.PostAsJsonAsync("/api/companies",
                companyGiven);
            Company? companyCreated = await httpResponseMessage.Content.ReadFromJsonAsync<Company>();
            CreateEmployeeRequest employeeGiven = new CreateEmployeeRequest { Name = "John" };
            await httpClient.PostAsJsonAsync($"/api/companies/{companyCreated?.Id}/employees", employeeGiven);

            // When
            httpResponseMessage = await httpClient.PostAsJsonAsync($"/api/companies/{companyCreated?.Id}/employees", employeeGiven);
            Employee? employeeCreated = await httpResponseMessage.Content.ReadFromJsonAsync<Employee>();

            // Then
            Assert.Null(employeeCreated?.Name);
            Assert.Equal(HttpStatusCode.BadRequest, httpResponseMessage.StatusCode);
        }

        [Fact]
        public async Task Should_return_bad_reqeust_when_create_employee_given_wrong_company_id_and_a_employeeName()
        {
            // Given
            CreateEmployeeRequest employeeGiven = new CreateEmployeeRequest { Name = "John" };

            // When
            var httpResponseMessage = await httpClient.PostAsJsonAsync($"/api/companies/{new Guid()}/employees", employeeGiven);
            Employee? employeeCreated = await httpResponseMessage.Content.ReadFromJsonAsync<Employee>();

            // Then
            Assert.Null(employeeCreated?.Name);
            Assert.Equal(HttpStatusCode.BadRequest, httpResponseMessage.StatusCode);
        }

        [Fact]
        public async Task Should_return_bad_reqeust_when_create_employee_given_a_companyid_with_unknown_employee_field()
        {
            // Given
            StringContent content = new StringContent("{\"unknownField\": \"BlueSky Digital Media\"}", Encoding.UTF8, "application/json");
            CreateCompanyRequest companyGiven = new CreateCompanyRequest { Name = "BlueSky Digital Media" };
            HttpResponseMessage httpResponseMessage = await httpClient.PostAsJsonAsync("/api/companies",
                companyGiven);
            Company? companyCreated = await httpResponseMessage.Content.ReadFromJsonAsync<Company>();

            // When
            httpResponseMessage = await httpClient.PostAsync($"/api/companies/{companyCreated?.Id}/employees", content);
            Employee? employeeCreated = await httpResponseMessage.Content.ReadFromJsonAsync<Employee>();

            // Then
            Assert.Null(employeeCreated?.Name);
            Assert.Equal(HttpStatusCode.BadRequest, httpResponseMessage.StatusCode);
        }

        [Fact]
        public async Task Should_return_deleted_employee_with_204_given_delete_with_companyId_and_employeeId()
        {
            // Given
            CreateCompanyRequest companyGiven = new CreateCompanyRequest { Name = "BlueSky Digital Media" };
            HttpResponseMessage httpResponseMessage = await httpClient.PostAsJsonAsync("/api/companies",
                companyGiven);
            Company? companyCreated = await httpResponseMessage.Content.ReadFromJsonAsync<Company>();
            CreateEmployeeRequest employeeGiven = new CreateEmployeeRequest { Name = "John" };
            httpResponseMessage = await httpClient.PostAsJsonAsync($"/api/companies/{companyCreated?.Id}/employees", employeeGiven);
            Employee? employeeCreated = await httpResponseMessage.Content.ReadFromJsonAsync<Employee>();

            // When
            httpResponseMessage = await httpClient.DeleteAsync($"/api/companies/{companyCreated?.Id}/employees/{employeeCreated?.Id}");
            Employee? employeeDeleted = await httpResponseMessage.Content.ReadFromJsonAsync<Employee>();

            // Then
            Assert.Equal(employeeCreated, employeeDeleted);
            Assert.Equal(HttpStatusCode.NoContent, httpResponseMessage.StatusCode);
        }

        [Fact]
        public async Task Should_return_nothing_with_notfound_given_delete_with_wrong_companyId_and_wrong_employeeId()
        {
            // Given

            // When
            var httpResponseMessage = await httpClient.DeleteAsync($"/api/companies/{new Guid()}/employees/{new Guid()}");
            Employee? employeeDeleted = await httpResponseMessage.Content.ReadFromJsonAsync<Employee>();

            // Then
            Assert.Null(employeeDeleted?.Name);
            Assert.Equal(HttpStatusCode.NotFound, httpResponseMessage.StatusCode);
        }
    }
}