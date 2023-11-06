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
        public async Task Should_return_created_company_with_status_201_when_create_cpmoany_given_a_company_name()
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
        public async Task Should_return_companies_with_status_ok_when_get_all_given_exists_companies()
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
        public async Task Should_return_nothing_with_status_ok_when_get_all_given_no_exist_companies()
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
            Company? compantReturnedFromGet = await httpResponseMessage.Content.ReadFromJsonAsync<Company>();

            //Then
            Assert.Equal(companyReturnedFromPost, compantReturnedFromGet);
            Assert.Equal(HttpStatusCode.OK, httpResponseMessage.StatusCode);
        }

        [Fact]
        public async Task Should_return_status_not_found_when_get_given_not_found_the_company_with_id()
        {
            //Given

            //When
            HttpResponseMessage message = await httpClient.GetAsync($"api/companies/{new Guid()}");
            Company? compantReturnedFromGet = await message.Content.ReadFromJsonAsync<Company>();

            //Then
            Assert.Null(compantReturnedFromGet?.Name);
            Assert.Equal(HttpStatusCode.NotFound, message.StatusCode);
        }

        [Fact]
        public async Task Should_return_companies_in_given_page_with_status_ok_when_get_all_given_exists_companies_and_pageSize_and_pageIndex()
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
        public async Task Should_return_nothing_with_status_ok_when_get_all_given_not_enought_companies_for_pageSize_and_pageIndex()
        {
            //Given

            //When
            HttpResponseMessage httpResponseMessage = await httpClient.GetAsync("/api/companies?pageSize=2&pageIndex=2");
            List<Company>? result = await httpResponseMessage.Content.ReadFromJsonAsync<List<Company>>();

            //Then
            Assert.Equal(0, result?.Count);
            Assert.Equal(HttpStatusCode.OK, httpResponseMessage.StatusCode);
        }
    }
}