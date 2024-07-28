using ImageTheatre.Data.Models;
using ImageTheatre.Data.Models.DTO;
using ImageTheatre.Data.Repositories;
using ImageTheatre.Data.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ImageTheatre.API.Controllers;

[Route("api/movies")]
[Authorize]
[ApiController]
public class MoviesController(IFileService _fileService,
    IMovieRepository _movieRepository, ILogger<MoviesController> logger) : ControllerBase
{

    /// <summary>
    ///  Create a Movie
    /// </summary>
    /// <response code="401">Unauthorized</response>
    /// <response code="201">Movie Created Successfully</response>
    [HttpPost]
    public async Task<IActionResult> CreateMovie([FromForm] MovieDto createMovieDto)
    {
        try
        {
            if (createMovieDto.MovieImageFile?.Length > 1 * 1024 * 1024)
            {
                return StatusCode(StatusCodes.Status400BadRequest, "File size should not exceed 1 MB");
            }

            string[] allowedFileExtentions = [".jpg", ".jpeg", ".png"];
            string createdImageName = await _fileService.SaveFileAsync(createMovieDto.MovieImageFile, allowedFileExtentions);

            var movie = new Movie
            {
                MovieTitle = createMovieDto.MovieTitle,
                ImdbRating = createMovieDto.ImdbRating,
                YearReleased = createMovieDto.YearReleased,
                Budget = createMovieDto.Budget,
                BoxOffice = createMovieDto.BoxOffice,
                Language = createMovieDto.Language,
                AuthorID = createMovieDto.AuthorID,
                MovieImage = createdImageName
            };

            var createdMovie = await _movieRepository.AddAsync(movie);
            return CreatedAtAction(nameof(CreateMovie), createdMovie);

        }

        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    /// <summary>
    /// Update a Movie
    /// </summary>

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateMovie(int id, [FromForm] UpdateMovieDto updateMovieDto)
    {
        try
        {
            if (id != updateMovieDto.MovieID)
            {
                return StatusCode(StatusCodes.Status400BadRequest, $"id in url and form body does not match.");
            }

            var existingMovie = await _movieRepository.GetByIdAsync(id);
            if (existingMovie == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, $"Movie with id: {id} does not found");
            }

            string oldImage = existingMovie.MovieImage;
            if (updateMovieDto.MovieImage != null)
            {
                if (updateMovieDto.MovieImage?.Length > 1 * 1024 * 1024)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, "File size should not exceed 1 MB");
                }
                string[] allowedFileExtentions = [".jpg", ".jpeg", ".png"];
                string createdImageName = await _fileService.SaveFileAsync(updateMovieDto.MovieImageFile, allowedFileExtentions);
                updateMovieDto.MovieImage = createdImageName;
            }

            existingMovie.MovieID = updateMovieDto.MovieID;
            existingMovie.MovieTitle = updateMovieDto.MovieTitle;
            existingMovie.ImdbRating = updateMovieDto.ImdbRating;
            existingMovie.YearReleased = updateMovieDto.YearReleased;
            existingMovie.Budget = updateMovieDto.Budget;
            existingMovie.BoxOffice = updateMovieDto.BoxOffice;
            existingMovie.Language = updateMovieDto.Language;
            existingMovie.AuthorID = updateMovieDto.AuthorID;

            var updatedMovie = await _movieRepository.UpdateAsync(existingMovie);

            // if image is updated, then we have to delete old image from directory
            if (updateMovieDto.MovieImage != null)
                _fileService.DeleteFile(oldImage);

            return Ok(updatedMovie);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    /// <summary>
    ///  Get a Movie by id
    /// </summary>

    [HttpGet("{id}")]
    public async Task<IActionResult> GetMovie(int id)
    {
        var movie = await _movieRepository.GetByIdAsync(id);
        if (movie == null)
        {
            return StatusCode(StatusCodes.Status404NotFound, $"Movie with id: {id} does not found");
        }
        return Ok(movie);
    }


    /// <summary>
    /// Delete a Movie
    /// </summary>

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMovie(int id)
    {
        try
        {
            var existingMovie = await _movieRepository.GetByIdAsync(id);
            if (existingMovie == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, $"Movie with id: {id} does not found");
            }

            await _movieRepository.DeleteAsync(existingMovie);
            _fileService.DeleteFile(existingMovie.MovieImage);
            return NoContent();  // return 204
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    /// <summary>
    ///  Get All Movies
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetMovies()
    {
        var movies = await _movieRepository.GetAllAsync();
        return Ok(movies);
    }


}
