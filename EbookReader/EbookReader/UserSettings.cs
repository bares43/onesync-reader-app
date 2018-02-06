using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using EbookReader.DependencyService;
using EbookReader.Provider;
using Microsoft.AppCenter.Analytics;
using Plugin.Settings;
using Plugin.Settings.Abstractions;
using Xamarin.Forms;

namespace EbookReader {
    public static class UserSettings {
        private static ISettings AppSettings => CrossSettings.Current;

        public static bool FirstRun {
            get => AppSettings.GetValueOrDefault(CreateKey(nameof(FirstRun)), true);
            set => AppSettings.AddOrUpdateValue(CreateKey(nameof(FirstRun)), value);
        }

        public static bool AnalyticsAgreement {
            get => AppSettings.GetValueOrDefault(CreateKey(nameof(AnalyticsAgreement)), false);
            set {
                SetAnalytics(value);
                AppSettings.AddOrUpdateValue(CreateKey(nameof(AnalyticsAgreement)), value);
            }
        }

        private async static void SetAnalytics(bool enabled) {
            await Analytics.SetEnabledAsync(enabled);
        }

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

            public static bool NightMode {
                get => AppSettings.GetValueOrDefault(CreateKey(nameof(Reader), nameof(NightMode)), false);
                set => AppSettings.AddOrUpdateValue(CreateKey(nameof(Reader), nameof(NightMode)), value);
            }

            public static bool Fullscreen {
                get => AppSettings.GetValueOrDefault(CreateKey(nameof(Reader), nameof(Fullscreen)), Device.RuntimePlatform == Device.Android);
                set => AppSettings.AddOrUpdateValue(CreateKey(nameof(Reader), nameof(Fullscreen)), value);
            }
        }

        public static class Synchronization {

            public static bool Enabled {
                get => AppSettings.GetValueOrDefault(CreateKey(nameof(Synchronization), nameof(Enabled)), true);
                set => AppSettings.AddOrUpdateValue(CreateKey(nameof(Synchronization), nameof(Enabled)), value);
            }

            public static long DeviceID {
                get {
                    var id = AppSettings.GetValueOrDefault(CreateKey(nameof(Synchronization), nameof(DeviceID)), default(long));
                    if (id == default(long)) {
                        id = DeviceIdProvider.ID;
                        AppSettings.AddOrUpdateValue(CreateKey(nameof(Synchronization), nameof(DeviceID)), id);
                    }
                    return id;
                }
            }

            public static string DeviceName {
                get => AppSettings.GetValueOrDefault(CreateKey(nameof(Synchronization), nameof(DeviceName)), DeviceNameProvider.Name);
                set => AppSettings.AddOrUpdateValue(CreateKey(nameof(Synchronization), nameof(DeviceName)), value);
            }

            public static string Service {
                get => AppSettings.GetValueOrDefault(CreateKey(nameof(Synchronization), nameof(Service)), SynchronizationServicesProvider.Dumb);
                set => AppSettings.AddOrUpdateValue(CreateKey(nameof(Synchronization), nameof(Service)), value);
            }

            public static bool OnlyWifi {
                get => AppSettings.GetValueOrDefault(CreateKey(nameof(Synchronization), nameof(OnlyWifi)), false);
                set => AppSettings.AddOrUpdateValue(CreateKey(nameof(Synchronization), nameof(OnlyWifi)), value);
            }

            public static class Dropbox {
                public static string AccessToken {
                    get => AppSettings.GetValueOrDefault(CreateKey(nameof(Synchronization), nameof(Dropbox), nameof(AccessToken)), "");
                    set => AppSettings.AddOrUpdateValue(CreateKey(nameof(Synchronization), nameof(Dropbox), nameof(AccessToken)), value);
                }
            }

            public static class Firebase {
                public static string Email {
                    get => AppSettings.GetValueOrDefault(CreateKey(nameof(Synchronization), nameof(Firebase), nameof(Email)), "");
                    set => AppSettings.AddOrUpdateValue(CreateKey(nameof(Synchronization), nameof(Firebase), nameof(Email)), value);
                }

                public static string Password {
                    get => AppSettings.GetValueOrDefault(CreateKey(nameof(Synchronization), nameof(Firebase), nameof(Password)), "");
                    set => AppSettings.AddOrUpdateValue(CreateKey(nameof(Synchronization), nameof(Firebase), nameof(Password)), value);
                }
            }

        }

        public static class Control {
            public static bool ClickEverywhere {
                get => AppSettings.GetValueOrDefault(CreateKey(nameof(Control), nameof(ClickEverywhere)), false);
                set => AppSettings.AddOrUpdateValue(CreateKey(nameof(Control), nameof(ClickEverywhere)), value);
            }

            public static bool DoubleSwipe {
                get => AppSettings.GetValueOrDefault(CreateKey(nameof(Control), nameof(DoubleSwipe)), false);
                set => AppSettings.AddOrUpdateValue(CreateKey(nameof(Control), nameof(DoubleSwipe)), value);
            }
        }

        private static string CreateKey(params string[] names) {
            return string.Join(".", names);
        }
    }
}
