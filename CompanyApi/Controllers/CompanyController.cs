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

        
        
    }
}
