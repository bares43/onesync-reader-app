using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbookReader.Model.WebViewMessages {
    public class Message {
        public string Action { get; set; }
        public object Data { get; set; }
        public bool IsSent { get; set; }
    }
}
