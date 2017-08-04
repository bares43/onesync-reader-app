using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbookReader.Helpers {
    public static class Base64Helper {
        public static string Encode(string text) {
            var bytes = Encoding.UTF8.GetBytes(text);
            return Convert.ToBase64String(bytes);
        }

        public static string Decode(string text) {
            var bytes = Convert.FromBase64String(text);
            return Encoding.UTF8.GetString(bytes, 0, bytes.Length);
        }
    }
}
