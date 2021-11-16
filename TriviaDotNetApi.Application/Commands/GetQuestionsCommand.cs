using CrossCutting.MessageHelpers;
using MediatR;
using TriviaDotNetApi.Application.Models;

namespace TriviaDotNetApi.Application
{
    public class GetQuestionsCommand : IRequest<Response>
    {
        public TriviaFilterModel Payload { get; set; }

        public GetQuestionsCommand()
        { }

        public GetQuestionsCommand(TriviaFilterModel payload)
        {
            this.Payload = payload;
        }
    }
}
