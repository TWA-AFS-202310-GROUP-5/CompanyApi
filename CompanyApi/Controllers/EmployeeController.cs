using Microsoft.AspNetCore.Mvc;

namespace CompanyApi.Controllers
{
    [Route("api/employees")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private static List<Employee> employees = new List<Employee>();
        [HttpPost("{companyId}")]
        public ActionResult<Company> Create([FromBody]CreateCompanyRequest request, string companyId)
        {
            if (employees.Exists(employee => employee.Name.Equals(request.Name)))
            {
                return BadRequest();
            }
            Employee companyCreated = new Employee(companyId, request.Name);
            employees.Add(companyCreated);
            return StatusCode(StatusCodes.Status201Created, companyCreated);
        }
        [HttpDelete]
        public void ClearData()
        {
            employees.Clear();
        }
    }
}
