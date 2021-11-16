using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TriviaDotNetApi.Domain.AggregatesModel
{
    public interface ITriviaSingleActionRepository
    {
        Task CreateAsync(TriviaItem item);

        Task CreateAsync(ICollection<TriviaItem> items);

        IEnumerable<TriviaItem> GetQuestionsAsync(TriviaFilter filter);

        Task SaveChangesAsync();
    }
}
