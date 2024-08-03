using TheatreApp.Data.Models;
using TheatreApp.Data.Models.Data;
using Microsoft.EntityFrameworkCore;

namespace TheatreApp.Data.Repositories;

public interface IAuthorRepository
{
    Task<Author> AddAsync(Author author);
    Task<Author> UpdateAsync(Author author);
    Task<IEnumerable<Author>> GetAllAsync();
    Task<Author> GetByIdAsync(int id);   
    Task DeleteAsync(Author author);
}

public class AuthorRepository(AppDbContext _context) : IAuthorRepository
{

    public async Task<Author> AddAsync(Author author)
    {
        _context.Authors.Add(author);
        await _context.SaveChangesAsync();
        return author;
    }

    public async Task<Author> UpdateAsync(Author author)
    {
        _context.Authors.Update(author);
        await _context.SaveChangesAsync();
        return author;
    }

    public async Task DeleteAsync(Author author)
    {

            _context.Authors.Remove(author);
            await _context.SaveChangesAsync();
    }

    public async Task<Author> GetByIdAsync(int id)
    {
        var author= await _context.Authors.Include(a => a.MovieAuthors).FirstOrDefaultAsync(a => a.AuthorID == id);
        return author;
    }

    public async Task<IEnumerable<Author>> GetAllAsync()
    {
        return await _context.Authors.Include(a => a.MovieAuthors).ToListAsync();
    }

}
