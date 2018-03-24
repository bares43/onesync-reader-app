using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbookReader.Exceptions {
    public class UnknownFileFormatException : Exception {
        public UnknownFileFormatException() : base() {

        }

        public UnknownFileFormatException(string format) : base($"Unknow format: {format}") {
        }
    }
}
