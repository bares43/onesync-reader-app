using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbookReader {
    public static class AppSettings {
        public static class Synchronization {
            public static class Firebase {
                public const string BaseUrl = "https://ebook-reader-b6053.firebaseio.com/";
                public const string ApiKey = "AIzaSyA4TOO3_Pa1kb_s6zjBMqpehPLrTk8SrLA";
            }
        }

        public static class AppCenter {
            public const string Android = "3409ff3e-0819-467d-b9a0-69f611f33bcb";
            public const string UWP = "737f997b-2a89-441d-8248-7f1f1fc889e3";
        }
    }
}
