using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Moq;
using NewsAPI.Business.Interfaces;
using NewsAPI.Data;
using HackerNewsAPI.Controllers;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace NewsAPI.Tests
{
    [TestClass]
    public class NewsStoriesControllerTests
    {
        private Mock<INewsStoryService> _newsStoryServiceMock;
        private NewsStoriesController _controller;
        [TestInitialize]
        public void Initialise()
        {
            _newsStoryServiceMock = new Mock<INewsStoryService>();
            _controller = new NewsStoriesController(_newsStoryServiceMock.Object);
        }

        [TestMethod]
        public async Task GetNewStories_ReturnsOkResult()
        {

            // Arrange
            var fakeResponse = new ApiResponse
            {
                TotalCount = 3,
                Stories = new List<NewsAPI.Data.NewsStory>
                    {
                    new NewsAPI.Data.NewsStory { By = "Google", Title = "Story 1" },
                    new NewsAPI.Data.NewsStory { By = "Google", Title = "Story 2" },
                    new NewsAPI.Data.NewsStory { By = "Google", Title = "Story 3" }
                    }
            };
            _newsStoryServiceMock.Setup(s => s.GetStories(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(fakeResponse);

            // Act
            var result = await _controller.GetNewStories() as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);

            var data = result.Value as ApiResponse;
            Assert.IsNotNull(data);
            Assert.AreEqual(3, data.TotalCount);
            Assert.AreEqual("Google", data.Stories.Select(s => s.By).FirstOrDefault());
        }
    }
}
