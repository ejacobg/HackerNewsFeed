using System.Collections.Generic;
using System.Threading.Tasks;
using HackerNewsFeed.Data;

namespace HackerNewsFeed.Models
{
    // IFeedService represents a service for managing feed items.
    public interface IFeedService
    {
        // Feed returns the entire feed, including non-subscribed items.
        Task<List<Item>> Feed();

        // Update pulls new data for at least the subscribed items. Non-subscribed items are not guaranteed to be updated.
        Task Update();

        // Subscribe subscribes to the item with the given ID.
        // If the item is not on the feed, pulls data for it and adds it to the feed.
        void Subscribe(int itemid);

        // Unsubscribe unsubscribes from the item with the given ID.
        // Unsubscribing from an item not on the feed is a no-op.
        void Unsubscribe(int itemid);
        
        // Clear marks all non-subscribed items as unsubscribed.
        Task Clear();
    }
}