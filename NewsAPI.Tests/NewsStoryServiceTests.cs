using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using NewsAPI.Business.Services;
using NewsAPI.Data;
using Newtonsoft.Json;

namespace NewsAPI.Tests
{
    [TestClass]
    public class NewsStoryServiceTests
    {
        private CustomMemoryCache _customMemoryCache;
        private Mock<IOptions<ConfigurationSettings>> _mockOptions;
        private NewsStoryService _newsStoryService;
        private HttpClient _httpClient;

        [TestInitialize]
        public void Setup()
        {
            _customMemoryCache = new CustomMemoryCache();
            _mockOptions = new Mock<IOptions<ConfigurationSettings>>();
            _httpClient = new HttpClient();

            var configSettings = new ConfigurationSettings
            {
                HackerNewsBaseUrl = "https://hacker-news.firebaseio.com/v0/"
            };

            _mockOptions.Setup(x => x.Value).Returns(configSettings);

            _newsStoryService = new NewsStoryService(_mockOptions.Object, _customMemoryCache);
        }

        [TestMethod]
        public void Constructor_ShouldInitializeFields()
        {
            // Arrange
            var cache = new MemoryCache(new MemoryCacheOptions());
            var mockOptions = new Mock<IOptions<ConfigurationSettings>>();
            mockOptions.Setup(m => m.Value).Returns(new ConfigurationSettings
            {
                HackerNewsBaseUrl = "https://hacker-news.firebaseio.com/v0/"
            });

            // Act
            var service = new NewsStoryService(mockOptions.Object, cache);

            // Assert
            Assert.IsNotNull(service);
        }
        [TestMethod]
        public async Task GetStories_ValidInput_ReturnsStories()
        {
            // Arrange
            var pageNumber = 1;
            var pageSize = 1;

            var storyIds = new List<int> { 1, 2, 3, 4, 5 };
            var stories = new List<NewsAPI.Data.NewsStory>
                {
                new NewsAPI.Data.NewsStory { Title = "Test Story 1", By = "Author 1", Url = "http://test.com/1" },
                new NewsAPI.Data.NewsStory { Title = "Test Story 2", By = "Author 2", Url = "http://test.com/2" },
                new NewsAPI.Data.NewsStory { Title = "Test Story 3", By = "Author 3", Url = "http://test.com/3" },
                new NewsAPI.Data.NewsStory { Title = "Test Story 4", By = "Author 4", Url = "http://test.com/4" },
                new NewsAPI.Data.NewsStory { Title = "Test Story 5", By = "Author 5", Url = "http://test.com/5" }
                };

            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
            "SendAsync",
            ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri.ToString().EndsWith("beststories.json")),
            ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(storyIds))
            });

            _httpClient = new HttpClient(mockHttpMessageHandler.Object);

            foreach (var story in stories)
            {
                _customMemoryCache.CreateEntry(story.Title).Value = story;
            }

            // Act
            var result = await _newsStoryService.GetStories(null, pageNumber, pageSize);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Stories.Count);
        }

    }
}
