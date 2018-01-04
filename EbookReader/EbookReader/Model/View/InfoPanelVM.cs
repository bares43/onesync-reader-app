using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using EbookReader.Model.Messages;
using EbookReader.Service;
using Xamarin.Forms;

namespace EbookReader.Model.View {
    public class InfoPanelVM : BaseVM {

        string _pages;

        public string Pages {
            get => _pages;
            set {
                _pages = value;
                OnPropertyChanged();
            }
        }

        string _clock;

        public string Clock {
            get => _clock;
            set {
                _clock = value;
                OnPropertyChanged();
            }
        }

        public string BackgroundColor {
            get {
                return UserSettings.Reader.NightMode ? "#181819" : "transparent";
            }
        }

        public string TextColor {
            get {
                return UserSettings.Reader.NightMode ? "#eff2f7" : "#000000";
            }
        }

        public InfoPanelVM() {
            IocManager.Container.Resolve<IMessageBus>().Subscribe<PageChange>(HandlePageChange);

            SetClock();

            Device.StartTimer(new TimeSpan(0, 0, 10), () => {
                SetClock();

                return true;
            });
        }

        private void HandlePageChange(PageChange msg) {
            Pages = $"{msg.CurrentPage} / {msg.TotalPages}";
        }

        private void SetClock() {
            Clock = DateTime.Now.ToString("HH:mm");
        }
    }
}
