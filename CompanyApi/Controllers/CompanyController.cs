﻿using Microsoft.AspNetCore.Mvc;

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
            if (hasCompany(request.Name))
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

        //[HttpGet]
        //public ActionResult<Company> Get(int id)
        //{
        //    //return StatusCode(StatusCodes.Status200OK, companies);
        //}

        [HttpGet]
        public ActionResult<List<Company>> GetAll()
        {
            return StatusCode(StatusCodes.Status200OK, companies);
        }

        private bool hasCompany(string name)
        {
            return companies.Exists(company => company.Name.Equals(name));
        }
    }
}
