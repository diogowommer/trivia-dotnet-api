using MediatR;
using System.Threading.Tasks;
using System.Threading;
using System;
using Microsoft.Extensions.Configuration;
using TriviaDotNetApi.Domain.AggregatesModel;
using AutoMapper;
using System.Collections.Generic;
using CrossCutting.MessageHelpers;

namespace TriviaDotNetApi.Application
{
    public class CreateTriviaCommandHandler : IRequestHandler<CreateTriviaCommand, Response>
    {
        private readonly IConfiguration _configuration;
        private readonly ITriviaSingleActionRepository _repository;
        private readonly IMapper _mapper;

        public CreateTriviaCommandHandler(IConfiguration configuration,
                                        ITriviaSingleActionRepository repository,
                                        IMapper mapper)
        {
            _configuration = configuration;
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<Response> Handle(CreateTriviaCommand request, CancellationToken cancellationToken)
        {
            await _repository.CreateAsync(_mapper.Map<ICollection<TriviaItem>>(request.Payload.results));

            await _repository.SaveChangesAsync();

            return new Response("Done");
        }
    }
}