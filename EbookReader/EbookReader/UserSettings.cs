using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using EbookReader.DependencyService;
using EbookReader.Provider;
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace EbookReader {
    public static class UserSettings {
        private static ISettings AppSettings => CrossSettings.Current;

        public static class Reader {
            private static int FontSizeDefault = 20;
            private static int MarginDefault = 30;
            private static int ScrollSpeedDefault = 200;

            public static int FontSize {
                get => AppSettings.GetValueOrDefault(CreateKey(nameof(Reader), nameof(FontSize)), FontSizeDefault);
                set => AppSettings.AddOrUpdateValue(CreateKey(nameof(Reader), nameof(FontSize)), value);
            }

            public static int Margin {
                get => AppSettings.GetValueOrDefault(CreateKey(nameof(Reader), nameof(Margin)), MarginDefault);
                set => AppSettings.AddOrUpdateValue(CreateKey(nameof(Reader), nameof(Margin)), value);
            }

            public static int ScrollSpeed {
                get => AppSettings.GetValueOrDefault(CreateKey(nameof(Reader), nameof(ScrollSpeed)), ScrollSpeedDefault);
                set => AppSettings.AddOrUpdateValue(CreateKey(nameof(Reader), nameof(ScrollSpeed)), value);
            }
        }

        public static class Synchronization {

            public static bool Enabled {
                get => AppSettings.GetValueOrDefault(CreateKey(nameof(Synchronization), nameof(Enabled)), true);
                set => AppSettings.AddOrUpdateValue(CreateKey(nameof(Synchronization), nameof(Enabled)), value);
            }

            public static string DeviceName {
                get => AppSettings.GetValueOrDefault(CreateKey(nameof(Synchronization), nameof(DeviceName)), DeviceNameProvider.Name);
                set => AppSettings.AddOrUpdateValue(CreateKey(nameof(Synchronization), nameof(DeviceName)), value);
            }

            public static string Service {
                get => AppSettings.GetValueOrDefault(CreateKey(nameof(Synchronization), nameof(Service)), "");
                set => AppSettings.AddOrUpdateValue(CreateKey(nameof(Synchronization), nameof(Service)), value);
            }

            public static class Dropbox {
                public static string AccessToken {
                    get => AppSettings.GetValueOrDefault(CreateKey(nameof(Synchronization), nameof(Dropbox), nameof(AccessToken)), "");
                    set => AppSettings.AddOrUpdateValue(CreateKey(nameof(Synchronization), nameof(Dropbox), nameof(AccessToken)), value);
                }
            }
        }

        private static string CreateKey(params string[] names) {
            return string.Join(".", names);
        }
    }
}
