using NewsAPI.Data;

namespace NewsAPI.Tests
{
    public class MockData
    {
        public static ApiResponse GetStories()
        {
            IEnumerable<Task<NewsAPI.Data.NewsStory>> tasks = Enumerable.Empty<Task<NewsAPI.Data.NewsStory>>();
            ApiResponse apiResponse = new ApiResponse()
            {
                Stories = new List<NewsAPI.Data.NewsStory>()
                {
                    new NewsAPI.Data.NewsStory()
                    {
                        By="Google",
                        Title="Google News",
                        Url="www.google.com"
                    },
                    new NewsAPI.Data.NewsStory()
                    {
                        By="Yahoo",
                        Title="Yahoo News",
                        Url="www.Yahoo.com"
                    },
                    new NewsAPI.Data.NewsStory()
                    {
                        By="Rediff",
                        Title="Rediff News",
                        Url="www.Rediff.com"
                    }
                },
                TotalCount = 3
            };
            return apiResponse;
        }
    }
}
