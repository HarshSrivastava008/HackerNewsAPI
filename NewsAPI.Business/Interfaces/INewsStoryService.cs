using NewsAPI.Data;
using System.Net.Http;
using System.Threading.Tasks;

namespace NewsAPI.Business.Interfaces
{
    public interface INewsStoryService
    {
        /// <summary>
        /// Get the newest stories from the feed.
        /// </summary>
        /// <param name="searchText"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        Task<ApiResponse> GetStories(string searchText, int pageNumber, int pageSize);
        /// <summary>
        /// Get the newest stories by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<HttpResponseMessage> GetStoryById(int id);
    }
}
