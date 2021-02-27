using Microsoft.EntityFrameworkCore;


namespace Wookie_Bookstore.Models
{
    public class BookContext : DbContext
    {
        public BookContext(DbContextOptions<BookContext> options)
            : base(options)
        {
            
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        public DbSet<Book> Books { get; set; }
    }
}
