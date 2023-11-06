using CompanyApi;
using Microsoft.AspNetCore.Http;
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
            Company companyGiven = new Company("BlueSky Digital Media");
            
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
        public async Task Should_return_all_companies_when_get_all_companies_given_without_query()
        {
            //given
            await ClearDataAsync();
            CreateCompanyRequest companyRequest = new CreateCompanyRequest { Name = "Google"};
            await httpClient.PostAsJsonAsync("/api/companies", companyRequest);
            //when
            HttpResponseMessage httpResponseMessage = await httpClient.GetAsync("/api/companies");
            List<Company> companies = await httpResponseMessage.Content.ReadFromJsonAsync<List<Company>>();
            //then
            Assert.NotNull(companies);
        }

        [Fact]
        public async Task Should_return_corresponding_company_when_get_company_by_id_given_companyId()
        {
            //given
            await ClearDataAsync();
            CreateCompanyRequest companyRequest = new CreateCompanyRequest { Name = "BlueSky Digital Media" };
            HttpResponseMessage httpResponseMessage1 = await httpClient.PostAsJsonAsync("api/companies", companyRequest);
            Company company1 = await httpResponseMessage1.Content.ReadFromJsonAsync<Company>();
            //when
            HttpResponseMessage httpResponseMessage2 = await httpClient.GetAsync($"/api/companies/{company1.Id}");
            Company company2 = await httpResponseMessage2.Content.ReadFromJsonAsync<Company>();
            //then
            Assert.Equal(HttpStatusCode.OK, httpResponseMessage2.StatusCode);
            Assert.Equal(company1.Id, company2.Id);
        }

        [Fact]
        public async Task Should_return_not_found_when_get_company_by_id_given_unknown_id()
        {
            //given
            await ClearDataAsync();
            CreateCompanyRequest companyRequest = new CreateCompanyRequest { Name = "BlueSky Digital Media" };
            HttpResponseMessage httpResponseMessage1 = await httpClient.PostAsJsonAsync("api/companies", companyRequest);
            string unknownId = "de678553-jh4787";
            //when
            HttpResponseMessage httpResponseMessage2 = await httpClient.GetAsync($"/api/companies/{unknownId}");
            //then
            Assert.Equal(HttpStatusCode.NotFound, httpResponseMessage2.StatusCode);
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