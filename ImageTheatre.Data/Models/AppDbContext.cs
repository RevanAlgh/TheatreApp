using Microsoft.EntityFrameworkCore;

namespace ImageTheatre.Data.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Movie> Movies { get; set; }
    public DbSet<Author> Authors { get; set; }
    public DbSet<MovieAuthor> MovieAuthors { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MovieAuthor>()
            .HasKey(ma => new { ma.MovieID, ma.AuthorID });

        modelBuilder.Entity<MovieAuthor>()
            .HasOne(ma => ma.Movie)
            .WithMany(m => m.MovieAuthors)
            .HasForeignKey(ma => ma.MovieID);

        modelBuilder.Entity<MovieAuthor>()
            .HasOne(ma => ma.Author)
            .WithMany(a => a.MovieAuthors)
            .HasForeignKey(ma => ma.AuthorID);

        modelBuilder.Entity<Movie>()
            .Property(m => m.Budget)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Movie>()
            .Property(m => m.BoxOffice)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Author>()
        .Property(a => a.AuthorID)
        .ValueGeneratedOnAdd();

        base.OnModelCreating(modelBuilder);
    }
    }

