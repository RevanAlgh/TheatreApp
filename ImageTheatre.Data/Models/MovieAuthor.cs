namespace ImageTheatre.Data.Models;

    public class MovieAuthor
    {
        public int MovieID { get; set; }
        public Movie Movie { get; set; }

        public int AuthorID { get; set; }
        public Author Author { get; set; }
    }

