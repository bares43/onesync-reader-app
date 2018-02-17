using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbookReader.Model.Messages {
    public class GoToPage {
        public int Page { get; set; }
        public bool Next { get; set; }
        public bool Previous { get; set; }
    }
}
