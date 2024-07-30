namespace ImageTheatre.Data.Models;

public class FileAttachment
{
    public int FileAttachmentID { get; set; }
    public string FilePath { get; set; }
    public string FileName { get; set; }
    public int MovieID { get; set; }
    public Movie Movie { get; set; }
}

