using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Routine.Api.DtoParameters;
using Routine.Api.Entities;
using Routine.Api.Models;
using Routine.Api.Services;

namespace Routine.Api.Controllers
{
    [ApiController]
    [Route("api/companies")]
    //[Route("api/[controller]")]   不建议，因为controller可能更改
    public class CompaniesController : ControllerBase
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IMapper _mapper;

        //public IActionResult Index()
        //{
        //    return View();
        //}
        public CompaniesController(ICompanyRepository companyRepository,IMapper mapper)
        {
            this._companyRepository = companyRepository ??
                                     throw new ArgumentNullException(nameof(companyRepository));
            //构造函数注入对象映射器
            this._mapper = mapper ??
                          throw new ArgumentNullException(nameof(mapper));
        }
        /// <summary>
        /// head和get类似，但head并不返回资源体
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HttpHead]
        public async Task<ActionResult<IEnumerable<CompanyDto>>> 
            GetCompanies([FromQuery]CompanyDtoParameters parameters)
        {
            var companies = await _companyRepository.GetCompaniesAsync(parameters);
            //return new JsonResult(companies);

            var companyDtos = _mapper.Map<IEnumerable<CompanyDto>>(companies);

            
            //return companyDtos;
            //return Ok();
            return Ok(companyDtos);//返回202
            //return NotFound(companies);//返回404
        }
        [HttpGet("{companyId}",Name =nameof(GetCompany))]    //api/Companies/{companyId}
        public async Task<ActionResult<CompanyDto>> GetCompany(Guid companyId)
        {
            //var exist = await companyRepository.CompanyExistAsync(companyId);
            //if (!exist)
            //{
            //    return NotFound();
            //}
            var company = await _companyRepository.GetCompanyAsync(companyId);
            if (company == null)
            {
                return NotFound();
            }


            return Ok(_mapper.Map<CompanyDto>(company));
        }

        /// <summary>
        /// HttpPost返回的是201
        /// </summary>
        /// <param name="company"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<CompanyDto>> CreateCompany(CompanyAddDto company)
        {
            //使用ApiController这个特性，会自动返回400
            //if(company == null)
            //{
            //    return BadRequest();    //返回400
            //}

            var entity = _mapper.Map<Company>(company);
            _companyRepository.AddCompany(entity);
            
            //当这一步使用完成后才添加到数据库中
            await _companyRepository.SaveAsync();

            var returnDto = _mapper.Map<CompanyDto>(entity);
            
            return CreatedAtRoute(nameof(GetCompany),new { companyId = returnDto.Id},returnDto);
        }
        [HttpOptions]
        public IActionResult GetCompaniesOptions()
        {
            Response.Headers.Add("Allow","GET,POST,OPTION");
            return Ok();
        }

        [HttpDelete("{companyId}")]
        public async Task<IActionResult> DeleteCompany(Guid companyId)
        {
            var companyEntity = await _companyRepository.GetCompanyAsync(companyId);

            if (companyEntity == null)
            {
                return NotFound();
            }

            await _companyRepository.GetEmployeesAsync(companyId, null, null);

            _companyRepository.DeleteCompany(companyEntity);

            await _companyRepository.SaveAsync();

            return NoContent();
        }
    }
}
