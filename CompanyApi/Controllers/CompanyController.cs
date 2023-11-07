using Microsoft.AspNetCore.Mvc;

namespace CompanyApi.Controllers
{
    [Route("api/companies")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private static List<Company> companies = new List<Company>();
        private static Dictionary<string, List<Employee>> companyEmployeesMap = new Dictionary<string, List<Employee>>();

        [HttpPost]
        public ActionResult<Company> Create(CreateCompanyRequest request)
        {
            if (HasCompanyName(request.Name))
            {
                return BadRequest();
            }
            Company companyCreated = new Company(request.Name);
            companies.Add(companyCreated);
            return StatusCode(StatusCodes.Status201Created, companyCreated);
        }

        [HttpDelete]
        public void ClearData()
        {
            companies.Clear();
            companyEmployeesMap.Clear();
        }

        [HttpGet("{id}")]
        public ActionResult<Company> Get(string id)
        {
            Company? company = GetCompanyById(id);
            if (company is null)
            {
                return NotFound();
            }
            return Ok(companies.First(company => company.Id.Equals(id)));
        }

        [HttpGet]
        public ActionResult<List<Company>> GetCompanyByPage([FromQuery] int pageSize = 0, [FromQuery] int pageIndex = 0)
        {
            var returnCompanies = pageSize > 0 ? companies.Skip(pageSize * (pageIndex - 1)).Take(pageSize) : companies;
            return Ok(returnCompanies);
        }

        [HttpPut("{id}")]
        public ActionResult<Company> UpdateCompany(string id, [FromBody] CreateCompanyRequest request)
        {
            Company? company = GetCompanyById(id);
            if (company is null)
            {
                return NotFound();
            }

            company.Name = request.Name;

            return Ok(company);
        }

        private bool HasCompanyName(string name)
        {
            return companies.Exists(company => company.Name.Equals(name));
        }

        private Company? GetCompanyById(string id)
        {
            return companies.Find(company => company.Id.Equals(id));
        }

        [HttpPost("{companyId}/employees")]
        public ActionResult<Company> Create([FromRoute] string companyId, CreateEmployeeRequest request)
        {
            Company? company = GetCompanyById(companyId);
            if (company is null)
            {
                return BadRequest();
            }

            if(!companyEmployeesMap.ContainsKey(companyId))
            {
                companyEmployeesMap[companyId] = new List<Employee>();
            }

            if(companyEmployeesMap[companyId].Find(employee => employee.Name == request.Name) is not null)
            {
                return BadRequest();
            }

            Employee employeeCreated = new Employee(request.Name, request.Salary);
            companyEmployeesMap[companyId].Add(employeeCreated);
            return StatusCode(StatusCodes.Status201Created, employeeCreated);
        }

        [HttpDelete("{companyId}/employees/{employeeId}")]
        public ActionResult<Company> DeleteEmployee([FromRoute] string companyId, [FromRoute] string employeeId)
        {
            Company? company = GetCompanyById(companyId);
            if (company is null)
            {
                return NotFound();
            }

            if (!companyEmployeesMap.ContainsKey(companyId))
            {
                return NotFound();
            }

            Employee? employee = companyEmployeesMap[companyId].Find(employee => employee.Id == employeeId);
            if (employee is null)
            {
                return NotFound();
            }
            else
            {
                companyEmployeesMap[companyId].Remove(employee);
                return StatusCode(StatusCodes.Status204NoContent, employee);
            }
        }
    }
}
