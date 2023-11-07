using Microsoft.AspNetCore.Mvc;

namespace CompanyApi.Controllers
{
    [Route("api/companies")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private static List<Company> Companies = new List<Company>();





        [HttpPost]
        public ActionResult<Company> Create(CreateCompanyRequest request)
        {
            if (Companies.Exists(company => company.Name.Equals(request.Name)))
            {
                return BadRequest();
            }
            Company companyCreated = new Company(request.Name);
            Companies.Add(companyCreated);
            return StatusCode(StatusCodes.Status201Created, companyCreated);
        }

        [HttpGet("{id}")]
        public ActionResult<Company> GetOne(string id)
        {
            var company = Companies.FirstOrDefault(company => company.Id == id);

            return company == null ? StatusCode(StatusCodes.Status404NotFound) : StatusCode(StatusCodes.Status200OK, company);
        }

        [HttpGet]
        public ActionResult<List<Company>> GetAll()
        {
            return StatusCode(StatusCodes.Status200OK, Companies);
        }

        [HttpPut("{id}")]
        public ActionResult Put(UpdateCompanyRequest company, string id)
        {
            var result = Companies.FirstOrDefault(company => company.Id == id);
            if (result == null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
            result.Name = company.Name;
            return StatusCode(StatusCodes.Status204NoContent);
        }

        [HttpGet("{size}/{page}")]
        public ActionResult<List<Company>> GetPage(string size, string page)
        {
            var pageNumber = int.Parse(page);
            var pageSize = int.Parse(size);
            var result = Companies.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            return StatusCode(StatusCodes.Status200OK, result);
        }

        [HttpPost("{companyId}")]
        public  ActionResult<Employee> CreateEmployee(CreateEmployeeRequest employee, string companyId)
        {
            if (!Companies.Exists(company => company.Id.Equals(companyId)))
            {
                return BadRequest();
            }

            Employee e = new Employee(employee.Name);
            var company =Companies.FirstOrDefault(com => com.Id == companyId);
            company.Employees.Add(e);
            return StatusCode(StatusCodes.Status201Created, e);
        }


        [HttpDelete("{companyId}/{employeeId}")]
        public ActionResult DeleteEmployee(string companyId, string employeeId)
        {
            var company = Companies.FirstOrDefault(com => com.Id == companyId);
            company?.Employees.RemoveAll(emp => emp.Id == employeeId);
            return StatusCode(StatusCodes.Status200OK);
        }

    }
}
