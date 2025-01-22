using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace WebApplication1.Controllers
{
    public class PagesController : Controller
    {
        private readonly IWebHostEnvironment _env;

        public PagesController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [HttpGet("/login")]
        public IActionResult Login()
        {
            var filePath = Path.Combine(_env.WebRootPath, "pages", "login.html");
            return PhysicalFile(filePath, "text/html");
        }

        [HttpGet("/register")]
        public IActionResult Register()
        {
            var filePath = Path.Combine(_env.WebRootPath, "pages", "register.html");
            return PhysicalFile(filePath, "text/html");
        }
       [HttpGet("/sklep")]
public IActionResult sklep(int id)
{
    var filePath = Path.Combine(_env.WebRootPath, "pages", "sklep.html");
    
    if (!System.IO.File.Exists(filePath))
    {
        return NotFound("Plik sklep.html nie został znaleziony.");
    }

    return PhysicalFile(filePath, "text/html");
}
[HttpGet("/zamowienia")]
        public IActionResult Zamowienia()
        {
            var filePath = Path.Combine(_env.WebRootPath, "pages", "zamowienia.html");
            return PhysicalFile(filePath, "text/html");
        }
    }
}
