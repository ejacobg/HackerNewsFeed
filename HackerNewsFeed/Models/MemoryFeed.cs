using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using HackerNewsFeed.Data;

namespace HackerNewsFeed.Models
{
    // MemoryFeed saves feed information in memory.
    public class MemoryFeed : IFeedService
    {
        private readonly Dictionary<int, Item> _feed = new Dictionary<int, Item>();
        private readonly ReaderWriterLockSlim _feedLock = new ReaderWriterLockSlim();
        private readonly IFeedProvider _provider;
        
        public MemoryFeed(HttpClient client)
        {
            for (var i = 0; i < 5; i++)
            {
                _feed.Add(i, new Item
                {
                    ItemId = i,
                    Title = $"Item {i}",
                    Url = $"/item/{i}",
                    Points = i,
                    Created = DateTime.Now,
                    Updated = DateTime.Now,
                    Subscribed = null
                });
            }

            _provider = new ApiProvider(client);
        }

        public Task<List<Item>> Feed()
        {
            return Task.FromResult(_feed.Values.ToList());
        }

        public async Task Update()
        {
            // Needs to be put outside the locked section or you may run into issues.
            // See: https://www.graymatterdeveloper.com/2019/12/05/async-sync/
            var updates = await _provider.Pull();
            
            _feedLock.EnterWriteLock();

            var now = DateTime.Now;
            
            foreach (var item in _feed.Values)
            {
                item.Points++;
                item.Updated = now;
            }

            foreach (var update in updates)
            {
                if (_feed.TryGetValue(update.ItemId, out var item))
                {
                    item.Title = update.Title;
                    item.Url = update.Url;
                    item.Points = update.Points;
                    item.Created = update.Created;
                    item.Updated = now;
                }
                else
                {
                    update.Updated = now;
                    _feed.Add(update.ItemId, update);
                }
            }
            
            _feedLock.ExitWriteLock();
        }

        public void Subscribe(int id)
        {
            if (_feed.TryGetValue(id, out var item))
            {
                item.Subscribed = true;
            }
            else
            {
                _feed.Add(id, new Item
                {
                    ItemId = id,
                    Title = $"Item {id}",
                    Url = $"/item/{id}",
                    Points = id,
                    Updated = DateTime.Now,
                    Subscribed = true
                });
            }
        }

        public void Unsubscribe(int id)
        {
            if (_feed.TryGetValue(id, out var item))
            {
                item.Subscribed = false;
            }
        }

        public Task Clear()
        {
            _feedLock.EnterWriteLock();

            foreach (var item in _feed.Values)
            {
                item.Subscribed ??= false;
            }
            
            _feedLock.ExitWriteLock();

            return Task.CompletedTask;
        }
    }
}