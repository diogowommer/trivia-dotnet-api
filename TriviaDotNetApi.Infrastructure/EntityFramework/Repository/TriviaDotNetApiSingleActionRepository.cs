using TriviaDotNetApi.Domain.AggregatesModel;
using TriviaDotNetApi.Infrastructure.EntityFramework.Context;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace TriviaDotNetApi.Infrastructure.EntityFramework.Repository
{
    public class TriviaDotNetApiSingleActionRepository : ITriviaSingleActionRepository
    {
        public readonly EFDbContext context;

        public TriviaDotNetApiSingleActionRepository(EFDbContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task CreateAsync(TriviaItem item) =>
            await context.TriviaItem.AddAsync(item);

        public async Task CreateAsync(ICollection<TriviaItem> items) =>
            await context.TriviaItem.AddRangeAsync(items);

        public IEnumerable<TriviaItem> GetQuestionsAsync(TriviaFilter filter) =>
            context.TriviaItem.Where(x => x.difficulty == filter.difficulty).Take(filter.amount).AsEnumerable();

        public async Task SaveChangesAsync() => 
            await context.SaveChangesAsync();
    }
}
