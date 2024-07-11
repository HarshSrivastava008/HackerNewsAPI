using Microsoft.AspNetCore.Mvc;
using NewsAPI.Business.Interfaces;

namespace HackerNewsAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class NewsStoriesController : ControllerBase
    {
        private readonly INewsStoryService _newsStoryService;
        public NewsStoriesController(INewsStoryService newsStoryService)
        {
            _newsStoryService = newsStoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetNewStories(int pageNumber = 1, int pageSize = 200, string searchString = "")
        
        {
            var data = await _newsStoryService.GetStories(Convert.ToString(searchString), Convert.ToInt32(pageNumber), Convert.ToInt32(pageSize));
            return Ok(data);
        }
    }
}
