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

        [HttpGet("range")]
        public ActionResult<List<Company>> GetCompanyByRange([FromQuery(Name = "pageSize")]int pageSize, [FromQuery(Name = "pageIndex")] int pageIndex)
        {
            if (pageSize < 0 || pageIndex < 1)
            {
                return StatusCode(StatusCodes.Status200OK, new List<Company>());
            }
            int startBound = (pageIndex - 1) * pageSize;
            int endBound = Math.Min(startBound + pageSize, companies.Count()) - startBound;
            if (startBound >= companies.Count())
            {
                return StatusCode(StatusCodes.Status200OK, new List<Company>());
            }
            try
            {            
                List<Company> companyList = companies.GetRange(startBound, endBound);
                return StatusCode(StatusCodes.Status200OK, companyList);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status200OK, new List<Company>());
            }

        }

        [HttpPut("{id}")]
        public ActionResult Update([FromBody]UpdateCompanyRequest updateCompany, string id)
        {
            Company company = companies.FirstOrDefault(c => c.Id == id);
            if (company == null)
            {
                return NotFound();
            }
            companies.FirstOrDefault(c => c.Id == id).Name = updateCompany.Name;
            return StatusCode(StatusCodes.Status204NoContent);
        }

        [HttpDelete]
        public void ClearData()
        { 
            companies.Clear();
        }
    }
}
