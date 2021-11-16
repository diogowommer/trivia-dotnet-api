using System;


namespace TriviaDotNetApi.Domain.AggregatesModel
{
    public class TriviaFilter
    {
        public int amount { get; protected set; }
        public string difficulty { get; protected set; }
        public string type { get; protected set; }
    }
}
