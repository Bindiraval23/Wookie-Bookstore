using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wookie_Bookstore.Models
{
    public class Book
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public double Price { get; set; }
        public byte[] CoverPhoto { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string GUID { get; set; }

    }
}
