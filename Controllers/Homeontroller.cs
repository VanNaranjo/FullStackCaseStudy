using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CaseStudy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Homeontroller : ControllerBase
    {
        [HttpGet]
        public ActionResult<string> Index()
        {
            return "If you see this the server is up and running!";
        }
    }
}
 