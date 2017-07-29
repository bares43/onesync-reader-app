using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EbookReader.Exceptions.Epub;
using EbookReader.Model;

namespace EbookReader.Helpers {
    public static class EpubVersionHelper {
        public static EpubVersion ParseVersion(string version) {
            switch (version) {
                case "2.0":
                    return EpubVersion.V200;
                case "3.0":
                    return EpubVersion.V300;
                case "3.0.1":
                    return EpubVersion.V301;
                default:
                    throw new UnknownEpubVersionException(version);
            }
        }
    }
}
