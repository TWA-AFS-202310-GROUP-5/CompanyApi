using CompanyApi;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        private string companyUri = "/api/companies";
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

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        public async Task Should_return_all_company_when_getAll_given_exist_sevral_company(int companyNum)
        {
            await ClearDataAsync();
            List<CreateCompanyRequest> givenCompanyList = new List<CreateCompanyRequest>();
            for (int i = 0; i < companyNum; i++)
            {
                CreateCompanyRequest companyTemp = new CreateCompanyRequest { Name = "Test" + i };
                givenCompanyList.Add(companyTemp);
                HttpResponseMessage httpResponseMessageTemp = await httpClient.PostAsJsonAsync(companyUri, companyTemp);
            }

            List<Company> resultCompanyList = await httpClient.GetFromJsonAsync<List<Company>>(companyUri);

            Assert.Equal(givenCompanyList.Select(x => x.Name), resultCompanyList.Select(y => y.Name));


        }
        [Fact]
        public async Task Should_return_empty_list_when_getAll_given_no_exist_company()
        {
            await ClearDataAsync();

            List<Company> resultCompanyList = await httpClient.GetFromJsonAsync<List<Company>>(companyUri);

            Assert.Empty(resultCompanyList);

        }

        [Fact]
        public async Task Should_return_404_NOTFOUND_when_get_company_given_no_exist_Id()
        {
            await ClearDataAsync();
            List<CreateCompanyRequest> givenCompanyList = new List<CreateCompanyRequest>();
            CreateCompanyRequest companyTemp = new CreateCompanyRequest { Name = "Test" };
            givenCompanyList.Add(companyTemp);
            HttpResponseMessage httpResponseMessage1 = await httpClient.PostAsJsonAsync(companyUri, companyTemp);

            string fakeId = "fake id 111";
            HttpResponseMessage httpResponseMessage2 = await httpClient.GetAsync(companyUri + "/" + fakeId);

            Assert.Equal(HttpStatusCode.BadRequest, httpResponseMessage2.StatusCode);

        }

        [Fact]
        public async Task Should_return_200OK_and_company_when_get_company_given_correct_Id()
        {
            await ClearDataAsync();
            List<CreateCompanyRequest> givenCompanyList = new List<CreateCompanyRequest>();
            CreateCompanyRequest companyTemp = new CreateCompanyRequest { Name = "Test" };
            givenCompanyList.Add(companyTemp);
            HttpResponseMessage httpResponseMessage1 = await httpClient.PostAsJsonAsync(companyUri, companyTemp);
            Company company = await httpResponseMessage1.Content.ReadFromJsonAsync<Company>();

            HttpResponseMessage httpResponseMessage2 = await httpClient.GetAsync(companyUri + "/" + company.Id);
            Company company1 = await httpResponseMessage2.Content.ReadFromJsonAsync<Company>();

            Assert.Equal(HttpStatusCode.OK, httpResponseMessage2.StatusCode);
            Assert.Equal(company.Name, company1.Name);
            Assert.Equal(company.Id, company1.Id);

        }

        [Theory]
        [InlineData(6, 2, 4)]
        [InlineData(6, 3, 3)]
        public async Task Should_return_empty_list_when_getCompanyByRange_given_invalid_start_index(int companyNum, int pageSize, int pageIndex)
        {
            await ClearDataAsync();

            for (int i = 0; i < companyNum; i++)
            {
                CreateCompanyRequest companyTemp = new CreateCompanyRequest { Name = "Test" + i };
                HttpResponseMessage httpResponseMessageTemp = await httpClient.PostAsJsonAsync(companyUri, companyTemp);
            }
            List<Company> resultCompanyList = await httpClient.GetFromJsonAsync<List<Company>>($"{companyUri}/range?pageSize={pageSize}&pageIndex={pageIndex}");

            Assert.Empty(resultCompanyList);

        }

        [Theory]
        [InlineData(5, 2, 3)]
        [InlineData(6, 1, 3)]
        public async Task Should_return_correct_list_when_getCompanyByRange_given_valid_index(int companyNum, int pageSize, int pageIndex)
        {
            await ClearDataAsync();

            for (int i = 0; i < companyNum; i++)
            {
                CreateCompanyRequest companyTemp = new CreateCompanyRequest { Name = "Test" + i };
                HttpResponseMessage httpResponseMessageTemp = await httpClient.PostAsJsonAsync(companyUri, companyTemp);
            }
            List<Company> resultCompanyList = await httpClient.GetFromJsonAsync<List<Company>>($"{companyUri}/range?pageSize={pageSize}&pageIndex={pageIndex}");

            Assert.Equal(1, resultCompanyList.Count());

        }

        [Fact]
        public async Task Should_return_404_NOTFOUND_when_update_company_given_no_exist_Id()
        {
            await ClearDataAsync();
            CreateCompanyRequest companyCreate = new CreateCompanyRequest { Name = "Test" };
            HttpResponseMessage httpResponseMessage1 = await httpClient.PostAsJsonAsync(companyUri, companyCreate);

            string fakeId = "fake id 111";
            UpdateCompanyRequest updateCompany = new UpdateCompanyRequest { Name = "Test-Updated" };
            HttpResponseMessage httpResponseMessage2 = await httpClient.PutAsJsonAsync(companyUri + "/" + fakeId, updateCompany);

            Assert.Equal(HttpStatusCode.NotFound, httpResponseMessage2.StatusCode);

        }

        [Fact]
        public async Task Should_return_204_NO_Content_when_update_company_given_exist_Id()
        {
            await ClearDataAsync();
            CreateCompanyRequest companyCreate = new CreateCompanyRequest { Name = "Test" };
            HttpResponseMessage httpResponseMessageCreate = await httpClient.PostAsJsonAsync(companyUri, companyCreate);
            Company correctCompany = await httpResponseMessageCreate.Content.ReadFromJsonAsync<Company>();
            correctCompany.Name = "Test-Updated";

            UpdateCompanyRequest updateCompany = new UpdateCompanyRequest { Name = "Test-Updated" };
            HttpResponseMessage httpResponseMessagePut = await httpClient.PutAsJsonAsync(companyUri + "/" + correctCompany.Id, updateCompany);
            HttpResponseMessage httpResponseMessageGet = await httpClient.GetAsync(companyUri + "/" + correctCompany.Id);
            Company resultCompany = await httpResponseMessageGet.Content.ReadFromJsonAsync<Company>();

            Assert.Equal(HttpStatusCode.NoContent, httpResponseMessagePut.StatusCode);
            Assert.Equal(correctCompany.Name, resultCompany.Name);
            Assert.Equal(correctCompany.Id, resultCompany.Id);

        }
        [Fact]
        public async Task Should_return_created_employee_when_add_employee_to_exist_company()
        {
            await ClearDataAsync();
            CreateCompanyRequest companyCreate = new CreateCompanyRequest { Name = "Test" };
            HttpResponseMessage httpResponseMessageCreate = await httpClient.PostAsJsonAsync(companyUri, companyCreate);
            Company company = await httpResponseMessageCreate.Content.ReadFromJsonAsync<Company>();
            CreateEmployeeRequest givenEmployee = new CreateEmployeeRequest {  Name = "Wang", Salary = 6000 };
            
            HttpResponseMessage httpResponseMessageAddEmp = await httpClient.PostAsJsonAsync($"{companyUri}/{company.Id}", givenEmployee);
            Employee createdEmployee = await httpResponseMessageAddEmp.Content.ReadFromJsonAsync<Employee>();
            
            Assert.NotNull(createdEmployee);
            Assert.NotNull(createdEmployee.Id);
            Assert.Equal(givenEmployee.Name, createdEmployee.Name);

        }

        [Fact]
        public async Task Should_return_404_NOTFOUND_when_add_employee_to_nonexist_company()
        {
            await ClearDataAsync();
            CreateEmployeeRequest givenEmployee = new CreateEmployeeRequest { Name = "Wang", Salary = 6000 };
            string fakeCompanyId = "111";
            
            HttpResponseMessage httpResponseMessageAddEmp = await httpClient.PostAsJsonAsync($"{companyUri}/{fakeCompanyId}", givenEmployee);


            Assert.Equal(HttpStatusCode.NotFound, httpResponseMessageAddEmp.StatusCode);

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