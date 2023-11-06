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
            Assert.Equal(HttpStatusCode.Created, httpResponseMessage1.StatusCode);
            Assert.Equal(HttpStatusCode.Created, httpResponseMessage2.StatusCode);

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