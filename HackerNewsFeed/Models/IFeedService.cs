using System.Collections.Generic;

namespace HackerNewsFeed.Models
{
    // IFeedService represents a service for managing feed items.
    public interface IFeedService
    {
        // Feed returns the entire feed, including non-subscribed items.
        IEnumerable<Item> Feed();

        // Update pulls new data for at least the subscribed items. Non-subscribed items are not guaranteed to be updated.
        void Update();

        // Subscribe subscribes to the item with the given ID.
        void Subscribe(int id);

        // Unsubscribe unsubscribes from the item with the given ID.
        void Unsubscribe(int id);
        
        // Clear marks all non-subscribed items as unsubscribed.
        void Clear();
    }
}