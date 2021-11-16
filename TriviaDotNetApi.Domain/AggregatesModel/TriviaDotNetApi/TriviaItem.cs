using System;


namespace TriviaDotNetApi.Domain.AggregatesModel
{
    public class TriviaItem
    {
        public virtual Guid Id { get; protected set; }
        public string category { get; protected set; }
        public string type { get; protected set; }
        public string difficulty { get; protected set; }
        public string question { get; protected set; }
        public string correct_answer { get; protected set; }

        internal void ResetId() =>
            this.Id = default;
    }
}
