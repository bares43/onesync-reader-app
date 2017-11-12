using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using EbookReader.DependencyService;
using EbookReader.Service;
using EbookReader.Model.Messages;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using EbookReader.Model.View;

namespace EbookReader.Page.Reader.QuickPanelTab {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Settings : StackLayout {

        private IMessageBus _messageBus;

        public Settings() {

            // IOC
            _messageBus = IocManager.Container.Resolve<IMessageBus>();

            InitializeComponent();

            BindingContext = new QuickPanelSettingsVM();

            if (Device.RuntimePlatform == Device.Android) {
                FontPicker.WidthRequest = 75;
                FontPicker.Title = "Písmo";

                MarginPicker.WidthRequest = 75;
                MarginPicker.Title = "Odsazení";

                var brightnessProvider = IocManager.Container.Resolve<IBrightnessProvider>();
                Brightness.Value = brightnessProvider.Brightness * 100;
            }

            if (Device.RuntimePlatform == Device.UWP) {
                SectionLook.Remove(BrightnessWrapper);
            }

        }

        private void Brightness_ValueChanged(object sender, ValueChangedEventArgs e) {
            if (e.OldValue != e.NewValue) {
                _messageBus.Send(new ChangesBrightness {
                    Brightness = (float)e.NewValue / 100
                });
            }
        }

    }
}