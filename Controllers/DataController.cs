using CaseStudy.DAL;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CaseStudy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataController : ControllerBase
    {
        readonly AppDbContext? _ctx;
        public DataController(AppDbContext context) // injected here
        {
            _ctx = context;
        }

        private static async Task<String> GetGpuJsonFromWebAsync()
        {
            string url = "https://raw.githubusercontent.com/VanNaranjo/FullStackCaseStudy/refs/heads/master/GPUData.json";
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(url);
            var result = await response.Content.ReadAsStringAsync();
            return result;
        }

        [HttpGet]
        public async Task<ActionResult<String>> Index()
        {
            DataUtility util = new(_ctx!);
            string payload = "";
            var json = await GetGpuJsonFromWebAsync();
            try
            {
                payload = (await util.LoadGpuInfoFromWebToDb(json)) ? "tables loaded" : "problem loading tables";
            }
            catch (Exception ex)
            {
                payload = ex.Message;
            }
            return JsonSerializer.Serialize(payload);
        }
    }
}
