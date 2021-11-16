using System;
using System.Collections.Generic;
using System.Linq;

namespace TriviaDotNetApi.Application.Models
{
    public class TriviaModel
    {
        public int response_code { get; set; }
        public ICollection<TriviaItemModel> results { get; set; }

    }
}