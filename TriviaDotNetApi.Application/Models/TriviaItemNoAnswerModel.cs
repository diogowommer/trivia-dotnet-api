using System;
using System.Collections.Generic;
using System.Linq;

namespace TriviaDotNetApi.Application.Models
{
    public class TriviaItemNoAnswerModel
    {
        public string category { get; set; }
        public string type { get; set; }
        public string difficulty { get; set; }
        public string question { get; set; }
    }
}