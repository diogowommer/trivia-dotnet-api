using CrossCutting.MessageHelpers;
using MediatR;
using TriviaDotNetApi.Application.Models;

namespace TriviaDotNetApi.Application
{
    public class CreateTriviaCommand : IRequest<Response>
    {
        public TriviaModel Payload { get; set; }

        public CreateTriviaCommand()
        { }

        public CreateTriviaCommand(TriviaModel payload)
        {
            this.Payload = payload;
        }
    }
}
