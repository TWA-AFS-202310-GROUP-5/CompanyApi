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
            Company companyGiven = new Company("BlueSky Digital Media");
            
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
            Company companyGiven = new Company("BlueSky Digital Media");

            // When
            HttpResponseMessage httpResponseMessage = await httpClient.PostAsJsonAsync("/api/companies",
                companyGiven);
            Company? companyCreated = await httpResponseMessage.Content.ReadFromJsonAsync<Company>();

            // Then
            Assert.Null(companyCreated?.Id);
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
            Assert.Null(companyCreated?.Id);
            Assert.Null(companyCreated?.Name);
            Assert.Equal(HttpStatusCode.BadRequest, httpResponseMessage.StatusCode);
        }

        [Fact]
        public async Task Should_return_companies_with_status_ok_when_get_all_given_exists_companies()
        {
            //Given
            Company companyGiven1 = new Company("BlueSky Digital Media");
            Company companyGiven2 = new Company("Google");
            await httpClient.PostAsJsonAsync("/api/companies", companyGiven1);
            await httpClient.PostAsJsonAsync("/api/companies", companyGiven2);

            //When
            HttpResponseMessage message = await httpClient.GetAsync("/api/companies");
            List<Company>? result = await message.Content.ReadFromJsonAsync<List<Company>>();

            //Then
            Assert.Equal(2, result?.Count);
            Assert.Equal(companyGiven1, result?[0]);
            Assert.Equal(companyGiven2, result?[1]);
            Assert.Equal(HttpStatusCode.OK, message.StatusCode);
        }

        [Fact]
        public async Task Should_return_nothing_with_status_ok_when_get_all_given_no_exist_companies()
        {
            //Given

            //When
            HttpResponseMessage message = await httpClient.GetAsync("/api/companies");
            List<Company>? result = await message.Content.ReadFromJsonAsync<List<Company>>();

            //Then
            Assert.Equal(0, result?.Count);
            Assert.Equal(HttpStatusCode.OK, message.StatusCode);
        }
    }
}