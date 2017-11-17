using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EbookReader.Model.Bookshelf;

namespace EbookReader.Model.Sync {
    public class Progress {
        public string BookID { get; set; }
        public string DeviceName { get; set; }
        public DateTime DateTime { get; set; }
        public Position Position { get; set; }
    }
}
