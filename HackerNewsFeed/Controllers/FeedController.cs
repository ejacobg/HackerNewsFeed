using Microsoft.AspNetCore.Mvc;

namespace HackerNewsFeed.Controllers
{
    public class FeedController : Controller
    {
        // GET
        public IActionResult Index()
        {
            return View();
        }
    }
}