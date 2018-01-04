using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EbookReader.Helpers {
    public static class PathHelper {
        public static string CombinePath(string path1, string path2) {
            string dummyDriveLetter = "C:/";
            string absolutePath1 = dummyDriveLetter + path1;

            var path1Uri = new Uri(absolutePath1, UriKind.Absolute);
            var path2Uri = new Uri(path2, UriKind.Relative);
            var diff = new Uri(path1Uri, path2Uri);
            return diff.OriginalString.Replace(dummyDriveLetter, "");
        }

        public static string NormalizePath(string path) {
            if (!string.IsNullOrEmpty(path)) {
                if (path.StartsWith("/")) {
                    path = path.Substring(1);
                }

                path = path.Replace("%20", " ");
                path = Regex.Replace(path, "/+", "/");
            }

            return path;
        }
    }
}
