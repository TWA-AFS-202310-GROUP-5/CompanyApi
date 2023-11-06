using Microsoft.AspNetCore.Mvc;

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

        [HttpGet]
        public ActionResult<List<Company>> GetAll()
        {
            return StatusCode(StatusCodes.Status200OK, companies);
        }

        [HttpGet("{id}")]
        public ActionResult<Company> GetCompanyById(string id)
        {
            Company company = companies.FirstOrDefault(x => x.Id == id);
            if (company == null)
            {
                return BadRequest();
            }
            return StatusCode(StatusCodes.Status200OK, company);
        }

        [HttpDelete]
        public void ClearData()
        { 
            companies.Clear();
        }
    }
}
