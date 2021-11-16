using System;
using System.Collections.Generic;
using System.Linq;

namespace TriviaDotNetApi.Application.Models
{
    public class TriviaFilterModel
    {
        public int amount { get; set; }
        public string difficulty { get; set; }
        public string type { get; set; }

    }
}