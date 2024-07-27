using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;


namespace ImageTheatre.Data.Models.DTO;

public class MovieDto 
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

    [Required]
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

    [Required]
    [MaxLength(50)]
    public string? MovieImage { get; set; }
    public IFormFile? MovieImageFile { get; set; }

}