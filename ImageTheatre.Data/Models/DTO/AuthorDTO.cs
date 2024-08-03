using System.ComponentModel.DataAnnotations;

namespace TheatreApp.Data.Models.DTO;

public class AuthorDto
{
    [Required]
    public int AuthorID { get; set; }
    [Required]
    [MaxLength(30)]
    public string AuthorName { get; set; }
}

public class CreateAuthorDto 
{
    [Required]
    [MaxLength(30)]
    public string AuthorName { get; set; }
}

public class UpdateAuthorDto 
{
    [Required]
    public int AuthorID { get; set; }

    [Required]
    [MaxLength(30)]
    public string AuthorName { get; set; }
}
