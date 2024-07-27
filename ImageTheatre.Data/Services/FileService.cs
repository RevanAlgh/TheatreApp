using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;


namespace ImageTheatre.Data.Services;
    
public interface IFileService
{
    Task<string> SaveFileAsync(IFormFile movieImage, string[] allowedFileExtensions);
    void DeleteFile(string fileNameWithExtension);
}

public class FileService(IWebHostEnvironment environment) : IFileService
{

    public async Task<string> SaveFileAsync(IFormFile movieImage, string[] allowedFileExtensions)
    {
        if (movieImage == null)
        {
            throw new ArgumentNullException(nameof(movieImage));
        }

        var contentPath = environment.ContentRootPath;
        var path = Path.Combine(contentPath, "Uploads");

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        // Check the allowed extenstions
        var ext = Path.GetExtension(movieImage.FileName);
        if (!allowedFileExtensions.Contains(ext))
        {
            throw new ArgumentException($"Only {string.Join(",", allowedFileExtensions)} are allowed.");
        }

        // generate a unique filename
        var fileName = $"{Guid.NewGuid().ToString()}{ext}";
        var fileNameWithPath = Path.Combine(path, fileName);
        using var stream = new FileStream(fileNameWithPath, FileMode.Create);
        await movieImage.CopyToAsync(stream);
        return fileName;
    }


    public void DeleteFile(string fileNameWithExtension)
    {
        if (string.IsNullOrEmpty(fileNameWithExtension))
        {
            throw new ArgumentNullException(nameof(fileNameWithExtension));
        }
        var contentPath = environment.ContentRootPath;
        var path = Path.Combine(contentPath, $"Uploads", fileNameWithExtension);

        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"Invalid file path");
        }
        File.Delete(path);
    }

}
