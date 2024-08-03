using TheatreApp.Data.Models;
using TheatreApp.Data.Models.Data;
using TheatreApp.Data.Models.DTO;
using TheatreApp.Data.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TheatreApp.API.Controllers;

[Route("api/movies")]
[ApiController]
public class MoviesController : ControllerBase
{
    private readonly IMovieRepository _movieRepository;
    private readonly ILogger<MoviesController> _logger;
    private readonly IFileAttachmentRepository _fileAttachmentRepository;


    public MoviesController(IMovieRepository movieRepository, ILogger<MoviesController> logger, IFileAttachmentRepository fileAttachmentRepository)
    {
        _movieRepository = movieRepository;
        _logger = logger;
        _fileAttachmentRepository = fileAttachmentRepository;
    }

    /// <summary>
    /// Get a Movie by id
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> GetMovie(int id)
    {
        try { 
        var movie = await _movieRepository.GetByIdAsync(id);
        if (movie == null)
        {
            return NotFound();
        }

        var movieDto = new MovieDto
        {
            MovieID = movie.MovieID,
            MovieTitle = movie.MovieTitle,
            ImdbRating = movie.ImdbRating,
            YearReleased = movie.YearReleased,
            Budget = movie.Budget,
            BoxOffice = movie.BoxOffice,
            Language = movie.Language,
            AuthorID = movie.AuthorID,
            MovieAuthors = movie.MovieAuthors.ToList(),
            ImageUrl = movie.FileAttachments?.FirstOrDefault()?.FilePath
        };

        if (!string.IsNullOrEmpty(movieDto.ImageUrl))
        {
            var imageFilePath = Path.Combine("Uploads", movieDto.ImageUrl);
            if (System.IO.File.Exists(imageFilePath))
            {
                var fileBytes = await System.IO.File.ReadAllBytesAsync(imageFilePath);
                var mimeType = "image/" + Path.GetExtension(imageFilePath).TrimStart('.');
                return File(fileBytes, mimeType);
            }
        }

        return Ok(movieDto);
    }
    catch (Exception ex)
        {
            _logger.LogError(ex, "Error");
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
}
    }


    /// <summary>
    /// Get Movie Image by id
    /// </summary>
    [HttpGet("{id}/image")]
    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> GetMovieImage(int id)
    {
        try
        {
            var movie = await _movieRepository.GetByIdAsync(id);
            if (movie == null || string.IsNullOrEmpty(movie.MovieImage))
            {
                return NotFound($"Image for movie with id: {id} not found");
            }

            var image = System.IO.File.OpenRead(movie.MovieImage);
        
        return File(image, "image/jpeg");
    }
            catch (Exception ex)
        {
            _logger.LogError(ex, "Error");
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
        }
    }

    /// <summary>
    /// Create a Movie
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateMovie([FromForm] CreateMovieDto createMovieDto)
    {
        try
        {
            string imagePath = null;
            if (createMovieDto.MovieImageFile != null)
            {
                if (createMovieDto.MovieImageFile.Length > 1 * 1024 * 1024)
                {
                    return BadRequest("File size should not exceed 1 MB");
                }

                var allowedFileExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var extension = Path.GetExtension(createMovieDto.MovieImageFile.FileName).ToLowerInvariant();
                if (Array.IndexOf(allowedFileExtensions, extension) < 0)
                {
                    return BadRequest("Invalid file extension");
                }

                var uploadsFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
                if (!Directory.Exists(uploadsFolderPath))
                {
                    Directory.CreateDirectory(uploadsFolderPath);
                }

                var fileName = $"{Guid.NewGuid()}{extension}";
                imagePath = Path.Combine(uploadsFolderPath, fileName);
                using (var fileStream = new FileStream(imagePath, FileMode.Create))
                {
                    await createMovieDto.MovieImageFile.CopyToAsync(fileStream);
                }
            }

            var movie = new Movie
            {
                MovieTitle = createMovieDto.MovieTitle,
                ImdbRating = createMovieDto.ImdbRating,
                YearReleased = createMovieDto.YearReleased,
                Budget = createMovieDto.Budget,
                BoxOffice = createMovieDto.BoxOffice,
                Language = createMovieDto.Language,
                AuthorID = createMovieDto.AuthorID,
                MovieImage = imagePath
            };

            var createdMovie = await _movieRepository.AddAsync(movie);
            return CreatedAtAction(nameof(GetMovie), new { id = createdMovie.MovieID }, createdMovie);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error");
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
        }
    }


    /// <summary>
    /// Update a Movie
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateMovie(int id, [FromForm] UpdateMovieDto updateMovieDto)
    {
        try
        {
            var movie = await _movieRepository.GetByIdAsync(id);
            if (movie == null)
            {
                return NotFound();
            }

            movie.MovieTitle = updateMovieDto.MovieTitle;
            movie.ImdbRating = updateMovieDto.ImdbRating;
            movie.YearReleased = updateMovieDto.YearReleased;
            movie.Budget = updateMovieDto.Budget;
            movie.BoxOffice = updateMovieDto.BoxOffice;
            movie.Language = updateMovieDto.Language;
            movie.AuthorID = updateMovieDto.AuthorID;

            if (updateMovieDto.MovieImageFile != null)
            {
                if (updateMovieDto.MovieImageFile.Length > 1 * 1024 * 1024)
                {
                    return BadRequest("File size should not exceed 1 MB");
                }

                var allowedFileExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var extension = Path.GetExtension(updateMovieDto.MovieImageFile.FileName).ToLowerInvariant();
                if (Array.IndexOf(allowedFileExtensions, extension) < 0)
                {
                    return BadRequest("Invalid file extension");
                }

                var uploadsFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
                if (!Directory.Exists(uploadsFolderPath))
                {
                    Directory.CreateDirectory(uploadsFolderPath);
                }

                var fileName = $"{Guid.NewGuid()}{extension}";
                var imagePath = Path.Combine(uploadsFolderPath, fileName);
                using (var fileStream = new FileStream(imagePath, FileMode.Create))
                {
                    await updateMovieDto.MovieImageFile.CopyToAsync(fileStream);
                }

                movie.MovieImage = imagePath;
                var fileAttachment = new FileAttachment
                {
                    FilePath = Path.GetFileName(imagePath),
                    FileName = Path.GetFileName(imagePath),
                    MovieID = movie.MovieID
                };
                await _fileAttachmentRepository.AddFileAttachmentAsync(fileAttachment);
            }

            await _movieRepository.UpdateAsync(movie);

            return Ok(movie);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error");
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
        }
    }

    /// <summary>
    /// Delete a Movie
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteMovie(int id)
    {
        try
        {
            await _movieRepository.DeleteAsync(id);
            return Ok(new { message = "Movie deleted successfully." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error");
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
        }
    }

    /// <summary>
    /// Get All Movies
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> GetMovies()
    {
        try
        {
            var movies = await _movieRepository.GetAllAsync();
            if (movies == null || !movies.Any())
            {
                return NotFound();
            }

            var movieDtos = movies.Select(movie => new MovieDto
            {
                MovieID = movie.MovieID,
                MovieTitle = movie.MovieTitle,
                ImdbRating = movie.ImdbRating,
                YearReleased = movie.YearReleased,
                Budget = movie.Budget,
                BoxOffice = movie.BoxOffice,
                Language = movie.Language,
                AuthorID = movie.AuthorID,
                MovieAuthors = movie.MovieAuthors.ToList(),
                ImageUrl = movie.FileAttachments?.FirstOrDefault()?.FilePath
            }).ToList();

            return Ok(movieDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error");
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
        }
    }



}
