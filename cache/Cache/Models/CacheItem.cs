using System;
using System.Collections.Generic;

namespace Cache.Models
{
    public class CacheItem
    {
        public string Request { get; set; }
        public List<Movie> Response { get; set; }
        public DateTime Timestamp { get; set; }
    }
}