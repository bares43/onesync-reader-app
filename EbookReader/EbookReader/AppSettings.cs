using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PCLAppConfig;

namespace EbookReader {
    public static class AppSettings {

        public static string Color = "#43A047";
        
        public static class Synchronization {
            public static class Firebase {
                public static string BaseUrl => ConfigurationManager.AppSettings["Firebase_BaseUrl"];
                public static string ApiKey => ConfigurationManager.AppSettings["Firebase_ApiKey"];
            }

            public static class Dropbox {
                public static string ClientID => ConfigurationManager.AppSettings["Dropbox_ClientID"];
                public static string RedirectUrl = "https://bares43.github.io/onesync-reader/oauth2_success.html";
            }
        }

        public static class AppCenter {
            public static string Android => ConfigurationManager.AppSettings["AppCenter_Android"];
            public static string UWP => ConfigurationManager.AppSettings["AppCenter_UWP"];
        }

        public static class Bookshelft {
            public static string SqlLiteFilename = "bookshelf.db3";
        }
    }
}
