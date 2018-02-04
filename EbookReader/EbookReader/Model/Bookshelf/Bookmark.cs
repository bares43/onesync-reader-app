using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbookReader.Model.Bookshelf {
    public class Bookmark {
        public long ID { get; set; }
        public string Name { get; set; }
        public Position Position { get; set; }
        public bool Deleted { get; set; }
        public DateTime LastChange { get; set; }
    }
}
