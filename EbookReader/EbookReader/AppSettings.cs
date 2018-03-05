using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbookReader {
    public static class AppSettings {

        public static string Color = "#00796B";
        
        public static class Synchronization {
            public static class Firebase {
                public static string BaseUrl => SecretSettings.Synchronization.Firebase.BaseUrl;
                public static string ApiKey => SecretSettings.Synchronization.Firebase.ApiKey;
            }

            public static class Dropbox {
                public static string ClientID => SecretSettings.Synchronization.Dropbox.ClientID;
                public static string RedirectUrl = "https://janbares.cz/www/onesync-reader/oauth2-success";
            }
        }

        public static class AppCenter {
            public static string Android => SecretSettings.AppCenter.Android;
            public static string UWP => SecretSettings.AppCenter.UWP;
        }

        public static class Bookshelft {
            public static string SqlLiteFilename = "bookshelf.db3";
        }
    }
}
