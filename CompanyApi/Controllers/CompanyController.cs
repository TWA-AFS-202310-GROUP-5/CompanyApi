using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Eventing.Reader;

namespace CompanyApi.Controllers
{
    [Route("api/companies")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private static List<Company> companies = new List<Company>();

        [HttpPost]
        public ActionResult<Company> Create(CreateCompanyRequest request)
        {
            if (companies.Exists(company => company.Name.Equals(request.Name)))
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
        }

        [HttpGet]
        public ActionResult<List<Company>> Get([FromQuery] string pageSize, [FromQuery] string pageIndex)
        {
            int pageSizeNum = int.Parse(pageSize);
            int pageIndexNum = int.Parse(pageIndex);
            if (companies.Count() < int.Parse(pageSize) * int.Parse(pageIndex))
            {
                return NotFound();
            }
            else
            {
                return StatusCode(StatusCodes.Status200OK, companies.Skip(pageSizeNum * (pageIndexNum - 1)).Take(pageSizeNum).ToList());
            }
        }

        [HttpGet("{id}")]
        public ActionResult<Company> Get(string id)
        {
            var company = companies.Where(c => c.Id == id).FirstOrDefault();
            if (company == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(company);
            }
        }

        /*
        [HttpGet]
        public ActionResult<List<Company>> Get()
        {
            return Ok(companies);
        }
        */

        [HttpPut("{id}")]
        public ActionResult<Company> Put(string id, CreateCompanyRequest company)
        {
            Company? existedCompany = companies.Where(c => c.Id == id).FirstOrDefault();

            if (existedCompany == null)
            {
                return NotFound();
            }
            else
            {
                existedCompany.Name = company.Name;
                return NoContent();
            }
        }

        [HttpPost("{companyId}/employees")]
        public ActionResult<Employee> AddEmployee(string companyId, EmployeeRequest employee)
        {
            Company? company = companies.FirstOrDefault(c => c.Id == companyId);
            if (company == null)
            {
                return NotFound();
            }
            else
            {
                Employee newEmployee = new Employee();
                newEmployee.Name = employee.Name;
                newEmployee.Salary = employee.Salary;
                company.Employees.Add(newEmployee);

                return Created("", newEmployee);
            }
        }

        [HttpDelete("{companyId}/employees/{employeeId}")]
        public ActionResult DeleteEmployee(string companyId, string employeeId)
        {
            var company = companies.FirstOrDefault(c => c.Id == companyId);
            var employee = company?.Employees.FirstOrDefault(e => e.Id == employeeId);
            if (employee == null)
            {
                return NotFound();
            }
            else
            {
                company.Employees.Remove(employee);
                return NoContent();
            }
        }

        [HttpGet("{companyId}/employees")]
        public ActionResult<List<Employee>> GetEmployeesByCompanyId(string companyId)
        {
            var company = companies.FirstOrDefault(c => c.Id == companyId);
            if (company == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(company.Employees);
            }
        }
    }
}
