using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Wookie_Bookstore.Models;
using static System.Net.Mime.MediaTypeNames;

namespace Wookie_Bookstore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly BookContext _DBContext;
        private Constants _constants;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IConfiguration _config;

        public BooksController(BookContext context, Constants constants, IWebHostEnvironment hostingEnvironment,
            IConfiguration config)
        {
            _DBContext = context;
            _constants = constants;
            _hostingEnvironment = hostingEnvironment;
            _config = config;
        }

        // GET: api/Books
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
        {
            var books = await _DBContext.Books.ToListAsync();
            if (books.Count == 0 && _constants.isFirstRun)
            {
                _constants.isFirstRun = false;
                AddDefaultBooks(); //inserts 2 default records
            }
            return await _DBContext.Books.ToListAsync();
        }

        // GET: api/Books/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBook(int id)
        {
            var book = await _DBContext.Books.FindAsync(id);

            if (book == null)
                return NotFound();

            return book;
        }

        // PUT: api/Books/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBook(int id, Book book)
        {
            if (id != book.ID)
                return BadRequest();

            if (book.CoverPhoto != null)
            {
                book.GUID = ImageUpload.WriteFile(book.FileName, book.CoverPhoto); //save new image to static folder
                book.CoverPhoto = null;
                book.FilePath = _config["ImagePath"] + book.GUID;
            }

            _DBContext.Entry(book).State = EntityState.Modified;

            try
            {
                await _DBContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // POST: api/Books
        [HttpPost]
        public async Task<ActionResult<Book>> PostBook(Book book)
        {
            if (book.CoverPhoto != null)
            {
                //save image to static folder
                book.GUID = ImageUpload.WriteFile(book.FileName, book.CoverPhoto);
                book.CoverPhoto = null;
                book.FilePath = _config["ImagePath"] + book.GUID;
            }

            _DBContext.Books.Add(book);
            await _DBContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetBook), new { id = book.ID }, book);

        }

        // DELETE: api/Books/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Book>> DeleteBook(int id)
        {
            var book = await _DBContext.Books.FindAsync(id);
            if (book == null)
                return NotFound();

            if (!string.IsNullOrEmpty(book.GUID))
                ImageUpload.DeleteFile(book.GUID);  //Delete image from static folder

            _DBContext.Books.Remove(book);
            await _DBContext.SaveChangesAsync();

            return book;
        }

        private bool BookExists(int id)
        {
            return _DBContext.Books.Any(e => e.ID == id);
        }

        private async void AddDefaultBooks()
        {
            _DBContext.Books.AddRange(new Book
            {
                ID = 1,
                Title = "The Secret of the Fortune Wookiee",
                Author = "Tom Angleberger",
                Price = 13.28,
                FileName = "Empire_strikes_back_old.jpg",
                FilePath = _config["ImagePath"] + "381784b9-0780-4f31-a13e-f3f20d7b3acf.jpg",
                GUID = "381784b9-0780-4f31-a13e-f3f20d7b3acf.jpg",
                CoverPhoto = null,
                Description = "With Dwight attending Tippett Academy this semester, the kids of McQuarrie Middle School are on their own—no Origami Yoda to give advice and help them navigate the treacherous waters of middle school."
            }, new Book
            {
                ID = 2,
                Title = "Light of the Jedi",
                Author = "Charles Soule",
                Price = 14.16,
                FileName = "JediSearch.jpg",
                FilePath = _config["ImagePath"] + "66d88d24-81e7-4a3c-a231-10e92e0ce6cd.jpg",
                GUID = "66d88d24-81e7-4a3c-a231-10e92e0ce6cd.jpg",
                CoverPhoto = null,
                Description = "The prequel trilogy revealed Master Yoda had been a key figure in the Jedi Order in the years before the Clone Wars. ... Yoda was apparently already considered one of three Jedi Grandmasters, but at the time of Light of the Jedi he had chosen to go on a sabbatical and train a group of Younglings."
            }); ;
            await _DBContext.SaveChangesAsync();
        }

    }
}
