using System;
using System.ComponentModel.DataAnnotations;

namespace HackerNewsFeed.Models
{
    // Item reflects that of a Hacker News item, but typically represents an story, Ask HN, or Show HN post.
    public class Item
    {
        public int ItemId { get; set; }
        public string Title { get; set; }

        // If this item contains a URL, then it will be added here.
        public string Url { get; set; }

        public int Points { get; set; }

        // Created represents the time this item was submitted to Hacker News, not when it was created in our local database.
        [DataType(DataType.Date)] public DateTime Created { get; set; }

        // Updated represents the time this item was last updated from the API.
        // Changing the subscription status shouldn't affect this timestamp.
        [DataType(DataType.Date)] public DateTime Updated { get; set; }

        // Subscribed is true if the user opened the item or explicitly subscribed to it, false if the user explicitly unsubscribed from it, and null if the user has yet to interact with this item.
        public bool? Subscribed { get; set; }

        // https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/statements-expressions-operators/expression-bodied-members#read-only-properties
        public string Discussion => $"https://news.ycombinator.com/item?id={ItemId}";
    }
}