using MediatR;
using TriviaDotNetApi.Application.Models;
using System.Threading.Tasks;
using System.Threading;
using System;
using Microsoft.Extensions.Configuration;
using TriviaDotNetApi.Domain.AggregatesModel;
using AutoMapper;
using Saunter.Attributes;
using System.Collections.Generic;
using CrossCutting.MessageHelpers;

namespace TriviaDotNetApi.Application
{
    [AsyncApi]
    public class GetQuestionsCommandHandler : IRequestHandler<GetQuestionsCommand, Response>
    {
        private readonly IConfiguration _configuration;
        private readonly ITriviaSingleActionRepository _repository;
        private readonly IMapper _mapper;

        public GetQuestionsCommandHandler(IConfiguration configuration,
                                        ITriviaSingleActionRepository repository,
                                        IMapper mapper)
        {
            _configuration = configuration;
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [PublishOperation(typeof(GetQuestionsCommand))]
        public async Task<Response> Handle(GetQuestionsCommand request, CancellationToken cancellationToken)
        {
            var result = _repository.GetQuestionsAsync(_mapper.Map<TriviaFilter>(request.Payload));

            return new Response(_mapper.Map<IEnumerable<TriviaItemModel>>(result));
        }
    }
}