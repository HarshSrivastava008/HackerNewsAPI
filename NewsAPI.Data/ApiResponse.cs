using System.Collections.Generic;

namespace NewsAPI.Data
{
    public class ApiResponse
    {
        public List<NewsStory> Stories { get; set; }
        public int TotalCount { get; set; }
    }
}
