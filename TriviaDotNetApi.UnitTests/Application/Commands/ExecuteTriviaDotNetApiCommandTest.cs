using AutoMapper;
using MediatR;
using Microsoft.Extensions.Configuration;
using Moq;
using TriviaDotNetApi.Application;
using TriviaDotNetApi.Application.Models;
using TriviaDotNetApi.Domain.AggregatesModel;
using System.Threading.Tasks;
using Xunit;
using System.Collections.Generic;

namespace TriviaDotNetApi.UnitTests.Application.Commands
{
    public class ExecuteTriviaDotNetApiCommandTest
    {
        [Fact]
        public async Task Success()
        {
            var command = new CreateTriviaCommand()
            {
                Payload = new TriviaModel()
                {
                    response_code = 123,
                    results = new List<TriviaItemModel> 
                    {
                        new TriviaItemModel
                        {
                            category = "Abc",
                            correct_answer = "Abc",
                            difficulty = "Abc",
                            question = "Abc",
                            type = "Abc",
                        }
                    }
                }
            };

            var configuration = new Mock<IConfiguration>().Object;
            var repository = new Mock<ITriviaSingleActionRepository>().Object;
            var mapper = new Mock<IMapper>().Object;

            var commandHandler = new CreateTriviaCommandHandler(configuration, repository, mapper);

            var commandReturn = await commandHandler.Handle(command, new System.Threading.CancellationToken());

            Assert.Equal("Done", commandReturn.Result);
        }
       
    }
}
