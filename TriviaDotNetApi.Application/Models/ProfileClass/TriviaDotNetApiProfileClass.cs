using AutoMapper;
using TriviaDotNetApi.Domain.AggregatesModel;

namespace TriviaDotNetApi.Application.Models.ProfileClass
{
    class TriviaDotNetApiProfileClass : Profile
    {
        public TriviaDotNetApiProfileClass()
        {
            CreateMap<TriviaItem, TriviaItemModel>().ReverseMap();
            CreateMap<TriviaItem, TriviaItemNoAnswerModel>().ReverseMap();
                        
            CreateMap<TriviaFilter, TriviaFilterModel>().ReverseMap();            
        }
    }
    
}
