using TheatreApp.Data.Models;
using TheatreApp.Data.Models.DTO;
using TheatreApp.Data.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TheatreApp.API.Controllers;

[Route("api/authors")]
[ApiController]
public class AuthorsController: ControllerBase
{ 
    private readonly IAuthorRepository _authorRepository;
    private readonly ILogger<AuthorsController> _logger;

    public AuthorsController(IAuthorRepository authorRepository, ILogger<AuthorsController> logger)
    {
        _authorRepository = authorRepository;
        _logger = logger;

    }

    /// <remarks>
    /// Sample Request:
    ///     POST
    ///     
    ///     {
    ///         "authorName": "Mario Amadeo"
    ///     }
    /// </remarks>

    /// <summary> Create an Author </summary>
    /// <response code="401">Unauthorized</response>
    /// <response code="201">Author Created Successfully</response>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateAuthor(CreateAuthorDto createAuthorDto)
    {
        try
        {
            var author = new Author
            {
                AuthorName = createAuthorDto.AuthorName
            };

            var createdAuthor = await _authorRepository.AddAsync(author);
            return CreatedAtAction(nameof(CreateAuthor), createdAuthor);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error");
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
        }
    }


    /// <summary>
    /// Update an Author
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateAuthor(int id, UpdateAuthorDto updateAuthorDto)
    {
        try
        {

            if (id != updateAuthorDto.AuthorID)
            {
                return StatusCode(StatusCodes.Status400BadRequest, $"id in url and form body does not match.");
            }

            var existingauthor = await _authorRepository.GetByIdAsync(id);
            if (existingauthor == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, $"Author with id: {id} does not found");
            }

            existingauthor.AuthorName = updateAuthorDto.AuthorName;

            var updatedAuthor = await _authorRepository.UpdateAsync(existingauthor);

            return Ok(updatedAuthor);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error");
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
        }
    }

    /// <summary>
    /// Get an Author by id
    /// </summary>
    /// 
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> GetAuthor(int id)
    {
        try
        {
            var author = await _authorRepository.GetByIdAsync(id);
            if (author == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, $"Author with id: {id} does not found");
            }

            return Ok(author);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error");
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
        }
    }

    /// <summary>
    /// Delete an Author
    /// </summary>

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteAuthor(int id)
    {
        try
        {
            var existingAuthor = await _authorRepository.GetByIdAsync(id);
            if (existingAuthor == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, $"Author with id: {id} does not found");
            }

            await _authorRepository.DeleteAsync(existingAuthor);
            return NoContent();  // return 204
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error");
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
        }
    }

    /// <summary>
    /// Get All Authors
    /// </summary>
    /// 
    [HttpGet]
    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> GetAuthors()
    {
        try
        {
            var authors = await _authorRepository.GetAllAsync();
            return Ok(authors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error");
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
        }
    }

}
