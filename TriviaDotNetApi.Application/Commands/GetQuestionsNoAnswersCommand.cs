using MediatR;
using TriviaDotNetApi.Application.Models;
using CrossCutting.MessageHelpers;

namespace TriviaDotNetApi.Application
{
    public class GetQuestionsNoAnswersCommand : IRequest<Response>
    {
        public TriviaFilterModel Payload { get; set; }

        public GetQuestionsNoAnswersCommand()
        { }

        public GetQuestionsNoAnswersCommand(TriviaFilterModel payload)
        {
            this.Payload = payload;
        }
    }
}
