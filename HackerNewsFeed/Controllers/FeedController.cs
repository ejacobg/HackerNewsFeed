using System.Threading.Tasks;
using HackerNewsFeed.Models;
using Microsoft.AspNetCore.Mvc;

namespace HackerNewsFeed.Controllers
{
    public class FeedController : Controller
    {
        private readonly IFeedService _feedService;

        public FeedController(IFeedService feedService)
        {
            _feedService = feedService;
        }

        // GET
        public async Task<IActionResult> Index()
        {
            await _feedService.Update();
            return View(_feedService.Feed());
        }

        // GET
        public IActionResult Refresh()
        {
            _feedService.Clear();
            return RedirectToAction("Index");
        }
    }
}