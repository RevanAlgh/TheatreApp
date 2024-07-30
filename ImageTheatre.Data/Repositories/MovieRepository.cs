using ImageTheatre.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ImageTheatre.Data.Repositories;

public interface IMovieRepository
{
    Task<Movie> AddAsync(Movie movie);
    Task<Movie> UpdateAsync(Movie movie);
    Task<IEnumerable<Movie>> GetAllAsync();
    Task<Movie?> GetByIdAsync(int id);
    Task DeleteAsync(int id);
}

public class MovieRepository : IMovieRepository
{
    private readonly AppDbContext _context;

    public MovieRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task<Movie> AddAsync(Movie movie)
    {
        _context.Movies.Add(movie);
        await _context.SaveChangesAsync();
        return movie;
    }

    public async Task<Movie> UpdateAsync(Movie movie)
    {
        _context.Movies.Update(movie);
        await _context.SaveChangesAsync();
        return movie;
    }

    public async Task DeleteAsync(int id)
    {
        var movie = await GetByIdAsync(id);
        if (movie != null)
        {
            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<Movie> GetByIdAsync(int id)
    {
        return await _context.Movies.FindAsync(id);
    }

    public async Task<IEnumerable<Movie>> GetAllAsync()
    {
        return await _context.Movies.ToListAsync();
    }

    public async Task AddFileAttachmentAsync(FileAttachment fileAttachment)
    {
        _context.FileAttachments.Add(fileAttachment);
        await _context.SaveChangesAsync();
    }

}
