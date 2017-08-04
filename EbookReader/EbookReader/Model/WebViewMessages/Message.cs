using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbookReader.Model.WebViewMessages {
    public class Message {
        public string Action { get; set; }
        public object Data { get; set; }
    }

    public class MyMessage {
        public const string Name = "MyMessage";

        public string P1 { get; set; }
        public int P2 { get; set; }
        public bool P3 { get; set; }
        public decimal P4 { get; set; }

    }

}
