using HackerNewsFeed.Models;
using Microsoft.AspNetCore.Mvc;

namespace HackerNewsFeed.Controllers
{
    public class ItemController : Controller
    {
        private readonly IFeedService _feedService;

        public ItemController(IFeedService feedService)
        {
            _feedService = feedService;
        }
        
        // GET
        public IActionResult Item(int id)
        {
            _feedService.Subscribe(id);
            return Redirect($"https://news.ycombinator.com/item?id={id}");
        }

        [HttpPost]
        public void Subscribe(int id)
        {
            _feedService.Subscribe(id);
        }

        [HttpPost]
        public void Unsubscribe(int id)
        {
            _feedService.Unsubscribe(id);
        }
    }
}