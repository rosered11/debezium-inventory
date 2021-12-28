using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.Text;

namespace InventoryManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VersionController : Controller
    {
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [HttpGet]
        public IActionResult GetVersion()
        {
            string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            return Content(version, "text/plain", Encoding.UTF8);
        }
    }
}
