using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TriviaDotNetApi.Application;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using CrossCutting.MessageHelpers;

namespace TriviaDotNetApi.API
{
    [ApiController]
    [Route("api/[controller]")]
    public class TriviaDotNetApiController : ControllerBase
    {
        private readonly IMediator mediator;

        public TriviaDotNetApiController(
            IMediator mediator
        )
        {
            this.mediator = mediator;
        }

        /// <summary>
        /// 1° step - Run createTriviaCommand to create a trivia.
        /// </summary>
        /// <param name="CreateQuestionsTrivia"></param>
        /// <returns></returns>
        [HttpPost("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Response>> CreateQuestions(CreateTriviaCommand createTriviaCommand) =>
               base.Ok(await this.mediator.Send(createTriviaCommand));

        /// <summary>
        /// 2° step - Run GetQuestionsTrivia to get a trivia list.
        /// </summary>
        /// <param name="GetQuestionsTrivia"></param>
        /// <returns></returns>
        [HttpGet("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Response>> GetQuestions([FromQuery] GetQuestionsCommand getQuestionsTriviaCommand) =>
            base.Ok(await this.mediator.Send(getQuestionsTriviaCommand));
        

        /// <summary>
        /// 3° step - Run GetQuestionsNoAnswersTrivia to get a trivia list.
        /// </summary>
        /// <param name="GetQuestionsTrivia"></param>
        /// <returns></returns>
        [HttpGet("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Response>> GetQuestionsNoAnswers([FromQuery] GetQuestionsNoAnswersCommand getQuestionsNoAnswersTrivia) =>
            base.Ok(await this.mediator.Send(getQuestionsNoAnswersTrivia));
        

    }
}