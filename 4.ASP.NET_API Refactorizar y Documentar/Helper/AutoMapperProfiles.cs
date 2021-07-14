using ASP.NET_API.DTOs;
using ASP.NET_API.DTOs.Accounts;
using ASP.NET_API.DTOs.Movie;
using ASP.NET_API.DTOs.Person;
using ASP.NET_API.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace ASP.NET_API.Helper
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Genre, GenreDTO>()
                .ReverseMap();

            CreateMap<Person, PersonDTO>()
                .ReverseMap();

            CreateMap<PersonCreationDTO, Person>()
                .ReverseMap();

            CreateMap<PersonCreationFormDTO, Person>()
                 .ForMember(x => x.Picture, options => options.Ignore()) //que no intente mapear la picture;
                 .ReverseMap();

            CreateMap<Movie, MovieDTO>()
                .ReverseMap();

            CreateMap<MovieCreationDTO, Movie>()

                .ForMember(x => x.Poster, options => options.Ignore()) //que no intente mapear la picture;
                .ForMember(x => x.MoviesGenres, opt => opt.MapFrom(MapMoviesGenres))
                .ForMember(x => x.MoviesActors, opt => opt.MapFrom(MapMoviesActors))
                            .ReverseMap();

            CreateMap<Movie, MovieDetailsDTO>()
            .ForMember(x => x.Genders, opt => opt.MapFrom(MapMoviesGenres))
            .ForMember(x => x.Actors, opt => opt.MapFrom(MapMoviesActors));


            CreateMap<IdentityUser, UserDTO>()
                .ForMember(x => x.EmailAdress, opt => opt.MapFrom(x => x.Email))
                .ForMember(x => x.UserID, opt => opt.MapFrom(x => x.Id))
                ;
                




        }

        #region MovieDetailsDto
        /// <summary>
        /// Mapea el elementos de MovieDetailsDTO.MoviesGenres a Movie
        /// </summary>
        /// <param name="movieCreationDTO"></param>
        /// <param name="movie"></param>
        /// <returns></returns>
        private List<GenreDTO> MapMoviesGenres(Movie movie, MovieDetailsDTO movieDetailsDTO)
        {
            var result = new List<GenreDTO>();
            foreach (var movieGenre in movie.MoviesGenres)
            {
                result.Add( new GenreDTO() { Id = movieGenre.GenreId , Name = movieGenre.Genre.Name });
            }

            return result;
        }

        /// <summary>
        /// Mapea el elementos de MovieDetailsDTO.MapMoviesActors a Movie
        /// </summary>
        /// <param name="movieDetailsDTO"></param>
        /// <param name="movie"></param>
        /// <returns></returns>
        private List<ActorDTO> MapMoviesActors(Movie movie, MovieDetailsDTO movieDetailsDTO)
        {
            var result = new List<ActorDTO>();
            foreach (var actor in movie.MoviesActors )
            {
                result.Add(new ActorDTO() { PersonId=actor.PersonId , Character=actor.Character , PersonName= actor.Person.Name });
            }

            return result;
        }

        #endregion


        #region movieCreatioDTO
        /// <summary>
        /// Mapea el elementos de MovieCreationDTO.MoviesGenres a Movie
        /// </summary>
        /// <param name="movieCreationDTO"></param>
        /// <param name="movie"></param>
        /// <returns></returns>
        private List<MoviesGenres> MapMoviesGenres(MovieCreationDTO movieCreationDTO, Movie movie)
        {
            var result = new List<MoviesGenres>();
            foreach (var id in movieCreationDTO.GendersId)
            {
                result.Add(new MoviesGenres() { GenreId = id });
            }

            return result;
        }


        /// <summary>
        /// Mapea el elementos de MovieCreationDTO.MoviesActors a Movie
        /// </summary>
        /// <param name="movieCreationDTO"></param>
        /// <param name="movie"></param>
        /// <returns></returns>
        private List<MoviesActors> MapMoviesActors(MovieCreationDTO movieCreationDTO, Movie movie)
        {
            var result = new List<MoviesActors>();
            foreach (var actor in movieCreationDTO.Actors)
            {
                result.Add(new MoviesActors() { PersonId = actor.PersonId, Character = actor.Character });
            }

            return result;
        }

        #endregion
    }
}