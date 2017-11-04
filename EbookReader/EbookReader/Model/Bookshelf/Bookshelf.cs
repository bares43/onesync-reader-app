using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbookReader.Model.Bookshelf {
    public class Bookshelf {
        public List<Book> Books { get; set; }

        public Bookshelf() {
            this.Books = new List<Book>();
        }
    }
}
