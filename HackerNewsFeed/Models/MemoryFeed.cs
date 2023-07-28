using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace HackerNewsFeed.Models
{
    // MemoryFeed saves feed information in memory.
    public class MemoryFeed : IFeedService
    {
        private Dictionary<int, Item> _feed = new Dictionary<int, Item>();
        private ReaderWriterLockSlim _feedLock = new ReaderWriterLockSlim();

        public MemoryFeed()
        {
            for (var i = 0; i < 5; i++)
            {
                _feed.Add(i, new Item
                {
                    ItemId = i,
                    Title = $"Item {i}",
                    Url = $"/item/{i}",
                    Points = i,
                    Updated = DateTime.Now,
                    Subscribed = null
                });
            }
        }

        public IEnumerable<Item> Feed()
        {
            return _feed.Values.ToList();
        }

        public void Update()
        {
            _feedLock.EnterWriteLock();

            foreach (var item in _feed.Values)
            {
                item.Points++;
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

        public void Clear()
        {
            _feedLock.EnterWriteLock();

            foreach (var item in _feed.Values)
            {
                item.Subscribed ??= false;
            }
            
            _feedLock.ExitWriteLock();
        }
    }
}