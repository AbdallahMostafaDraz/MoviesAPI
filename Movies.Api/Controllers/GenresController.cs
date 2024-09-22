using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Movies.Core.Constants;
using Movies.Core.Interfaces;
using Movies.Core.Models;
using Movies.EF.DTOs;

namespace Movies.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GenresController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public GenresController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

     
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllAsync()
        {
            var allGenres = await _unitOfWork.Genre.GetAllAsync();
            return Ok(allGenres);
        }

        [HttpGet("GetAllOrderd")]
        public async Task<ActionResult> GetAllOrderd(string? orderByDirection)
        {
            try
            {
                var allGenres = await _unitOfWork.Genre.GetAllAsync(null, e => e.Name, orderByDirection, null);
                return Ok(allGenres);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetOneById(int id)
        {
            var genre = await _unitOfWork.Genre.GetOneAsync(e => e.Id == id);
            if (genre == null)
                return NotFound("There no any genres with this id!");
            return Ok(genre);
        }

        [HttpPost("Add")]
        public async Task<IActionResult> Add(AddGenreDTO dto)
        {
            if (ModelState.IsValid)
            {
                var genre = new Genre { Name = dto.Name };
                await _unitOfWork.Genre.Add(genre);
                await _unitOfWork.Complete();

                return Ok(genre);
            }
            return BadRequest(ModelState);
        }


        [HttpPost("AddRange")]
        public async Task<IActionResult> AddRange(IEnumerable<AddGenreDTO> dtos)
        {
            if (ModelState.IsValid)
            {
                var genres = dtos.Select(dto => new Genre { Name = dto.Name }).ToList();

                await _unitOfWork.Genre.AddRange(genres);
                await _unitOfWork.Complete();

                return Ok(genres);
            }
            return BadRequest(ModelState);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, AddGenreDTO updateGenreDTO)
        {
            var genre = await _unitOfWork.Genre.GetOneAsync(e => e.Id == id);
            if (genre is null)
                return NotFound($"There No Genres With Id:[{id}]");
            
            genre.Name = updateGenreDTO.Name; 
            await _unitOfWork.Genre.Update(genre);
            await _unitOfWork.Complete();
            return Ok(genre);
            
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var genre = await _unitOfWork.Genre.GetOneAsync(e => e.Id == id);
            
            if (genre is null)
                return NotFound($"There No Genres With Id[{id}]!");

            await _unitOfWork.Genre.Delete(genre);
            await _unitOfWork.Complete();
            return Ok("Genre Deleted Successfully");
           
        }
    }
}