using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HackerNewsFeed.Data;
using Microsoft.EntityFrameworkCore;

namespace HackerNewsFeed.Models
{
    public class SqlServerFeed : IFeedService
    {
        private readonly FeedDbContext _context;
        private readonly IFeedProvider _provider;

        public SqlServerFeed(FeedDbContext context, HttpClient client)
        {
            _context = context;
            _provider = new ApiProvider(client);
        }

        public async Task<List<Item>> Feed()
        {
            return await _context.Items.AsNoTracking().ToListAsync();
        }

        public async Task Update()
        {
            var updates = await _provider.Pull();

            var feed = await _context.Items.ToDictionaryAsync(i => i.ItemId);

            var now = DateTime.Now;

            foreach (var update in updates)
            {
                if (feed.TryGetValue(update.ItemId, out var item))
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
                    feed.Add(update.ItemId, update);
                }
            }

            var needsUpdate = feed.Values.Where(item => item.Subscribed == true && item.Updated < now);
            var pullTasks = needsUpdate.Select(item => _provider.Pull(item));

            await Task.WhenAll(pullTasks);

            await _context.SaveChangesAsync();
        }

        public void Subscribe(int id)
        {
            var item = _context.Items.Find(id);
            if (item == null)
            {
                item = new Item { ItemId = id };
                _provider.Pull(item).Wait();
                _context.Items.Add(item);
            }

            item.Subscribed = true;

            _context.SaveChangesAsync();
        }

        public void Unsubscribe(int id)
        {
            var item = _context.Items.Find(id);
            if (item == null) return;
            item.Subscribed = false;
        }

        public async Task Clear()
        {
            var undecided = await _context.Items.Where(item => item.Subscribed == null).ToListAsync();

            foreach (var item in undecided)
            {
                item.Subscribed = false;
            }

            await _context.SaveChangesAsync();
        }
    }
}