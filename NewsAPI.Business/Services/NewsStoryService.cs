using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using NewsAPI.Business.Interfaces;
using NewsAPI.Data;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;

namespace NewsAPI.Business.Services
{
    public class NewsStoryService : INewsStoryService
    {
        private IMemoryCache _cache;
        private static HttpClient client = new HttpClient();
        private readonly ConfigurationSettings _config;
        public NewsStoryService(IOptions<ConfigurationSettings> config, IMemoryCache cache)
        {
            _config = config.Value;
            _cache = cache;
        }

        /// <summary>
        /// Get the newest stories from the feed.
        /// </summary>
        /// <param name="searchText"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<ApiResponse> GetStories(string searchText, int pageNumber, int pageSize)
        {
            IEnumerable<Task<NewsStory>> tasks = Enumerable.Empty<Task<NewsStory>>();
            ApiResponse apiResponse = new ApiResponse()
            {
                Stories = new List<NewsStory>()
            };
            if (pageNumber < 1)
                pageNumber = 1;

            if (pageSize < 1)
                pageSize = 200;

            List<NewsStory> stories = new List<NewsStory>();
            string url = string.Format("{0}{1}", _config.HackerNewsBaseUrl, "beststories.json");
            HttpResponseMessage response = await client.GetAsync(url);
            try
            {
                if (response.IsSuccessStatusCode)
                {
                    var storiesResponse = response.Content.ReadAsStringAsync().Result;
                    var bestIds = JsonConvert.DeserializeObject<List<int>>(storiesResponse);
                    if (bestIds != null)
                    {
                        int skip = (pageNumber - 1) * pageSize;
                        apiResponse.TotalCount = bestIds.Count;
                        if (String.IsNullOrEmpty(searchText))
                            tasks = bestIds.Skip(skip).Take(pageSize).Select(GetStoryAsync);
                        else
                            tasks = bestIds.Select(GetStoryAsync);
                        stories = (await Task.WhenAll(tasks)).ToList();

                        if (!String.IsNullOrEmpty(searchText))
                        {
                            var search = searchText.ToLower();
                            stories = stories.Where(s =>
                                               s.Title.ToLower().IndexOf(search) > -1 || s.By.ToLower().IndexOf(search) > -1)
                                               .Skip(skip).Take(pageSize).ToList();
                            apiResponse.TotalCount = stories.Count;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


            apiResponse.Stories.AddRange(stories);
            return apiResponse;
        }
        /// <summary>
        /// Get the newest story by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> GetStoryById(int id)
        {
            return await client.GetAsync(string.Format("{0}{1}", _config.HackerNewsBaseUrl, string.Format("item/{0}.json", id)));
        }

        /// <summary>
        /// Get the newest storiy by id.
        /// </summary>
        /// <param name="storyId"></param>
        /// <returns></returns>
        private async Task<NewsStory> GetStoryAsync(int storyId)
        {
            try
            {
                return await _cache.GetOrCreateAsync<NewsStory>(storyId, async cacheEntry =>
                {
                    var response = await GetStoryById(storyId);
                    if (response.IsSuccessStatusCode)
                    {
                        var storyResponse = await response.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<NewsStory>(storyResponse);
                    }

                    return new NewsStory();
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}
