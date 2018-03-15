using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbookReader.Model.Messages {
    public class ToastMessage {
        public string Message { get; set; }
        public PCLToastLength Length { get; set; }
    }

    public enum PCLToastLength {
        Short,
        Long,
    }
}
