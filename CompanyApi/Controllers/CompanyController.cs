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
        }

        [HttpGet("{id}")]
        public ActionResult<Company> Get(string id)
        {
            Company? company = GetCompanyById(id);
            if (company is null)
            {
                return NotFound();
            }
            return StatusCode(StatusCodes.Status200OK, companies.First(company => company.Id.Equals(id)));
        }

        [HttpGet]
        public ActionResult<List<Company>> GetAll()
        {
            return StatusCode(StatusCodes.Status200OK, companies);
        }

        private bool HasCompanyName(string name)
        {
            return companies.Exists(company => company.Name.Equals(name));
        }

        private Company? GetCompanyById(string id)
        {
            return companies.Where(company => company.Id.Equals(id)).FirstOrDefault();
        }
    }
}
