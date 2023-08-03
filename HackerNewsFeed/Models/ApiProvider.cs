using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using HackerNewsFeed.Data;

namespace HackerNewsFeed.Models
{
    public class ApiProvider : IFeedProvider
    {
        private readonly HttpClient _client;

        private const string AlgoliaUrl = "https://hn.algolia.com/api/v1/";
        private const string FirebaseUrl = "https://hacker-news.firebaseio.com/v0/";

        public ApiProvider(HttpClient client)
        {
            _client = client;
        }

        public async Task<List<Item>> Pull()
        {
            var frontPage =
                await _client.GetFromJsonAsync<AlgoliaResponse>(AlgoliaUrl + "search?tags=front_page&hitsPerPage=60");

            var cutoff = new DateTimeOffset(DateTime.Today.AddDays(-3));

            var askHn =
                await _client.GetFromJsonAsync<AlgoliaResponse>(AlgoliaUrl +
                                                                $"search?tags=ask_hn&hitsPerPage=90&numericFilters=created_at_i>{cutoff.ToUnixTimeSeconds()}");

            var updated = DateTime.Now;
            var items = new List<Item>();
            items.AddRange(frontPage.Hits.ConvertAll(model =>
            {
                model.Updated = updated;
                return model.ToItem();
            }));
            items.AddRange(askHn.Hits.ConvertAll(model =>
            {
                model.Updated = updated;
                return model.ToItem();
            }));

            return items;
        }

        public async void Pull(Item item)
        {
            var changes = await _client.GetFromJsonAsync<FirebaseModel>(FirebaseUrl + $"/item/{item.ItemId}.json");
            changes.UpdateItem(item);
        }

        public class AlgoliaResponse
        {
            public List<AlgoliaModel> Hits { get; set; }
        }

        public class AlgoliaModel
        {
            public string ObjectId { get; set; }
            public string Title { get; set; }
            public string Url { get; set; }
            public int Points { get; set; }
            public int Created { get; set; }

            [JsonPropertyName("num_comments")]
            public int? NumComments { get; set; }
            
            [JsonIgnore] public DateTime Updated { get; set; }

            public Item ToItem()
            {
                return new Item
                {
                    ItemId = Convert.ToInt32(ObjectId),
                    Title = Title,
                    Url = Url,
                    Points = Points,
                    Comments = NumComments ?? 0,
                    Created = DateTimeOffset.FromUnixTimeSeconds(Created).UtcDateTime,
                    Updated = Updated,
                };
            }
        }
        
        public class FirebaseModel
        {
            public string Title { get; set; }
            public string Url { get; set; }
            public int Score { get; set; }
            public int Created { get; set; }

            public int[] Kids { get; set; }
            public void UpdateItem(Item item)
            {
                item.Title = Title;
                item.Url = Url;
                item.Points = Score;
                item.Comments = Kids.Length;
                item.Created = DateTimeOffset.FromUnixTimeSeconds(Created).UtcDateTime;
                item.Updated = DateTime.Now;
            }
        }
    }
}