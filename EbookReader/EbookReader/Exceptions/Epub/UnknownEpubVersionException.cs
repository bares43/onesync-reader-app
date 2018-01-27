using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbookReader.Exceptions.Epub {
    public class UnknownEpubVersionException : Exception {
        public UnknownEpubVersionException() : base() {

        }

        public UnknownEpubVersionException(string version) : base($"Unknow epub version: {version}") {
        }
    }
}
