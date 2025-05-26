using CaseStudy.DAL;
using CaseStudy.DAL.DomainClasses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CaseStudy.DAL.DAO;

namespace CaseStudy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        readonly AppDbContext? _ctx;
        public ProductController(AppDbContext context) // injected here
        {
            _ctx = context;
        }

        [HttpGet]
        [Route("{braid}")]
        public async Task<ActionResult<List<Product>>> Index(int braid)
        {
            ProductDAO dao = new(_ctx!);
            List<Product> productsForBrand = await dao.GetAllByBrand(braid);
            return productsForBrand;
        }
    }
}
