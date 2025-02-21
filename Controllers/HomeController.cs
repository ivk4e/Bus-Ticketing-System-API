using Microsoft.AspNetCore.Mvc;

namespace BusifyAPI.Controllers
{
    public class HomeController
    {
        [HttpGet("error")]
        public IActionResult GetError()
        {
            throw new Exception("Това е тестова грешка!");
        }
    }
}
