using AutoMapper;
using Movies.Core.Models;
using Movies.Core.Models.Authentication;
using Movies.EF.DTOs;
using Movies.EF.DTOs.Authentication;

namespace Movies.Api.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            CreateMap<AddMovieDTO, Movie>().ForMember(src => src.Poster, opt => opt.Ignore());
            CreateMap<UpdateMovieDTO, Movie>().ForMember(src => src.Poster, opt => opt.Ignore());
            CreateMap<RegisterDTO, ApplicationUser>();
        }
    }
}
