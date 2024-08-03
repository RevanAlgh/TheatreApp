using Microsoft.AspNetCore.Http;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;


namespace TheatreApp.Data.Models.DTO;

public class MovieDto 
{
    [Required]
    public int MovieID { get; set; }

    [Required]
    [MaxLength(30)]
    public string MovieTitle { get; set; }
    public float ImdbRating { get; set; }
    public int YearReleased { get; set; }
    public decimal Budget { get; set; }
    public decimal BoxOffice { get; set; }
    public string Language { get; set; }
    public int AuthorID { get; set; }

    public string ImageUrl { get; set; }
    public ICollection<MovieAuthor> MovieAuthors { get; set; } = new Collection<MovieAuthor>();

}

public class CreateMovieDto
{
    [Required]
    [MaxLength(30)]
    public string MovieTitle { get; set; }
    public float ImdbRating { get; set; }
    public int YearReleased { get; set; }
    public decimal Budget { get; set; }
    public decimal BoxOffice { get; set; }
    public string Language { get; set; }
    public int AuthorID { get; set; }
    public IFormFile? MovieImageFile { get; set; }
}

public class UpdateMovieDto
{
    [Required]
    public int MovieID { get; set; }

    [Required]
    [MaxLength(30)]
    public string MovieTitle { get; set; }
    public float ImdbRating { get; set; }
    public int YearReleased { get; set; }
    public decimal Budget { get; set; }
    public decimal BoxOffice { get; set; }
    public string Language { get; set; }
    public int AuthorID { get; set; }
    public IFormFile? MovieImageFile { get; set; }



}