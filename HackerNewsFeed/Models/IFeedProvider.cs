using System.Collections.Generic;

namespace HackerNewsFeed.Models
{
    // IFeedProvider represents a service that can retrieve information about feed items.
    public interface IFeedProvider
    {
        // Pull retrieves new or updated items to be put on the feed. These items typically come from the Hacker News front page, but may be configured otherwise.
        IEnumerable<Item> Pull();

        // Pull attempts to retrieve data for the given item and writes it back into the object. If no updates were found, the timestamp will still be updated.
        void Pull(Item item);
    }
}