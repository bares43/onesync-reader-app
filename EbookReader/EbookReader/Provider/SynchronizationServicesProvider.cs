using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbookReader.Provider {
    public class SynchronizationServicesProvider {

        public const string Dumb = "Dumb";
        public const string Dropbox = "Dropbox";
        public const string Firebase = "E-mail";

        public static List<string> Items {
            get {
                return new List<string> {
                    Dropbox,
                    Firebase,
                };
            }
        }
    }
}
