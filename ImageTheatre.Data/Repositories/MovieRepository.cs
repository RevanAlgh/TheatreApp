using ImageTheatre.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ImageTheatre.Data.Repositories;

public interface IMovieRepository
{
    Task<Movie> AddAsync(Movie movie);
    Task<Movie> UpdateAsync(Movie movie);
    Task<IEnumerable<Movie>> GetAllAsync();
    Task<Movie?> GetByIdAsync(int id);
    Task DeleteAsync(Movie movie);
}

public class MovieRepository(AppDbContext _context) : IMovieRepository
{

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

    public async Task DeleteAsync(Movie movie)
    {
            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();
    }

    public async Task<Movie> GetByIdAsync(int id)
    {
        var movie = await _context.Movies.Include(m => m.MovieAuthors).FirstOrDefaultAsync(m => m.MovieID == id);
        return movie;
    }

    public async Task<IEnumerable<Movie>> GetAllAsync()
    {
        return await _context.Movies.Include(m => m.MovieAuthors).ToListAsync(); 
    }

    
}
