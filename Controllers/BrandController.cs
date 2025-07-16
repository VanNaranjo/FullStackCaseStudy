using CaseStudy.DAL;
using CaseStudy.DAL.DomainClasses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CaseStudy.DAL.DAO;
using Microsoft.AspNetCore.Authorization;

namespace CaseStudy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BrandController : ControllerBase
    {
        readonly AppDbContext? _ctx;
        public BrandController(AppDbContext context) // injected here
        {
            _ctx = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Brand>>> Index()
        {
            BrandDAO dao = new(_ctx!);
            List<Brand> allBrands = await dao.GetAll();

            return allBrands;
        }
    }
}
