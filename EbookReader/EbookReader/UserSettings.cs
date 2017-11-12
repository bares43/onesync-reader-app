using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using EbookReader.DependencyService;
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace EbookReader {
    public static class UserSettings {
        private static ISettings AppSettings => CrossSettings.Current;

        public static class Reader {
            private static int FontSizeDefault = 20;
            private static int MarginDefault = 30;

            public static int FontSize {
                get => AppSettings.GetValueOrDefault(CreateKey(nameof(Reader), nameof(FontSize)), FontSizeDefault);
                set => AppSettings.AddOrUpdateValue(CreateKey(nameof(Reader), nameof(FontSize)), value);
            }

            public static int Margin {
                get => AppSettings.GetValueOrDefault(CreateKey(nameof(Reader), nameof(Margin)), MarginDefault);
                set => AppSettings.AddOrUpdateValue(CreateKey(nameof(Reader), nameof(Margin)), value);
            }
        }

        private static string CreateKey(params string[] names) {
            return string.Join(".", names);
        }
    }
}
