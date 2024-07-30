using ImageTheatre.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ImageTheatre.Data.Repositories;

public interface IFileAttachmentRepository
{
    Task AddFileAttachmentAsync(FileAttachment fileAttachment);
    Task<FileAttachment> GetByMovieIdAsync(int movieId);
}


public class FileAttachmentRepository : IFileAttachmentRepository
{
    private readonly AppDbContext _context;

    public FileAttachmentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddFileAttachmentAsync(FileAttachment fileAttachment)
    {
        _context.FileAttachments.Add(fileAttachment);
        await _context.SaveChangesAsync();
    }

    public async Task<FileAttachment> GetByMovieIdAsync(int movieId)
    {
        return await _context.FileAttachments
            .FirstOrDefaultAsync(f => f.MovieID == movieId);
    }
}


