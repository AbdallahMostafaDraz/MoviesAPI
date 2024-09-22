using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Movies.Core.Interfaces;
using Movies.Core.Models;
using Movies.EF.DTOs;

namespace Movies.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MoviesController : ControllerBase
    {
        private IUnitOfWork _unitOfWork;
        private List<string> _allowedExtensions = new() { ".jpg", ".png" };
        private long _maxAllowedPosterSize = 1048576;
        private IMapper _mapper;
        public MoviesController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet("GetAll")]

        public async Task<IActionResult> GetAll()
        {
            var allMovies = await _unitOfWork.Movie.GetAllAsync(includeWords: new string[] { "Genre" });

            return Ok(allMovies);
        }

        [HttpGet("GetAllOrderd")]
        public async Task<IActionResult> GetAllOrderd()
        {
            var allMovies = await _unitOfWork.Movie.GetAllAsync(orderBy: e => e.Rate, OrderByDirection: "DES", includeWords: new string[] { "Genre" });
            return Ok(allMovies);
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var movie = await _unitOfWork.Movie.GetOneAsync(e => e.Id == id, new string[] {"Genre"});
            if (movie is null)
                return NotFound($"There No Any Movies With Id:[{id}]");
            return Ok(movie);
        }

        [HttpGet("GetByGenreId/{genreId}")]
        public async Task<IActionResult> GetByGenreId(byte genreId)
        {
            bool isValidGenre = await _unitOfWork.Genre.IsValid(genreId);

            if (!isValidGenre)
                return NotFound($"There No Genres With Id:[{genreId}]");

            var movies = await _unitOfWork.Movie.GetAllAsync(filter: e => e.GenreId == genreId, includeWords: new string[] { "Genre" });

            if (movies.Count() == 0)
                return NotFound("This Genre Has No Any Movies!");

            return Ok(movies);
        }

        private async Task<string> CheckInAdd(IFormFile poster, byte genreId)
        {
            if (_allowedExtensions.Contains(Path.GetExtension(poster.FileName.ToLower())))
                return "File Must Be With .jpg or .png Extenstion!";

            if (_maxAllowedPosterSize < poster.Length)
                return "File Must Be 1MB Maximum!";

            if (await _unitOfWork.Genre.IsValid(genreId))
                return $"There No Any Geners With Id:{genreId}";

            return string.Empty;

        }
        [HttpPost("Add")]
        public async Task<IActionResult> Add(AddMovieDTO addMovieDTO)
        {
            if (ModelState.IsValid)
            {
                if (!_allowedExtensions.Contains(Path.GetExtension(addMovieDTO.Poster.FileName.ToLower())))
                    return BadRequest("File Must Be With .jpg or .png Extenstion!");

                if (_maxAllowedPosterSize < addMovieDTO.Poster.Length)
                    return BadRequest("File Must Be 1MB Maximum!");

                if (!await _unitOfWork.Genre.IsValid(addMovieDTO.GenreId))
                    return BadRequest($"There No Any Geners With Id:{addMovieDTO.GenreId}");

                using var dataStream = new MemoryStream();
                await addMovieDTO.Poster.CopyToAsync(dataStream);

                var movie = _mapper.Map<Movie>(addMovieDTO);
                
                movie.Poster = dataStream.ToArray();

                await _unitOfWork.Movie.Add(movie);
                await _unitOfWork.Complete();

                return Ok(movie);
            }
            return BadRequest(ModelState);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] UpdateMovieDTO updateMovieDTO)
        {
            var movie = await _unitOfWork.Movie.GetOneAsync(e => e.Id == id);

            if (movie is null)
                return NotFound($"Threr No Moview With Id:[{id}]");

            if (!await _unitOfWork.Genre.IsValid(updateMovieDTO.GenreId))
                return BadRequest($"There No Any Geners With Id:{updateMovieDTO.GenreId}");


            if (updateMovieDTO.Poster is not null)
            {
                if (!_allowedExtensions.Contains(Path.GetExtension(updateMovieDTO.Poster.FileName.ToLower())))
                    return BadRequest("File Must Be With .jpg or .png Extenstion!");

                if (_maxAllowedPosterSize < updateMovieDTO.Poster.Length)
                    return BadRequest("File Must Be 1MB Maximum!");

                var dataStream = new MemoryStream();
                await updateMovieDTO.Poster.CopyToAsync(dataStream);
                movie.Poster = dataStream.ToArray();
            }

            _mapper.Map(updateMovieDTO, movie);

            await _unitOfWork.Movie.Update(movie);
            await _unitOfWork.Complete();

            return Ok(movie);
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var movie = await _unitOfWork.Movie.GetOneAsync(e => e.Id == id);

            if (movie is null)
                return NotFound($"There No Movies With Id[{id}]!");
            
            await _unitOfWork.Movie.Delete(movie);
            await _unitOfWork.Complete();
            return Ok(movie);
        }
        
    }
}
