using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using EbookReader.DependencyService;
using EbookReader.Service;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EbookReader.Page.Reader.QuickPanelTab {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Settings : StackLayout {

        public List<string> FontSizes {
            get {
                return new List<string> {
                    "12",
                    "14",
                    "16",
                    "18",
                    "20",
                    "22",
                    "24",
                    "26",
                    "28",
                    "30",
                    "32",
                    "34",
                    "36",
                    "38",
                    "40"
                };
            }
        }

        public List<string> Margins {
            get {
                return new List<string> {
                    "15",
                    "30",
                    "45",
                };
            }
        }

        public string DefaultFont = "20";
        public string DefaultMargin = "30";

        private IWebViewMessages _messages;
        private IMessageBus _messageBus;

        public event EventHandler OnSet;

        public Settings() {

            // IOC
            _messages = IocManager.Container.Resolve<IWebViewMessages>();
            _messageBus = IocManager.Container.Resolve<IMessageBus>();

            InitializeComponent();

            FontPicker.ItemsSource = this.FontSizes;
            FontPicker.SelectedItem = this.DefaultFont;

            MarginPicker.ItemsSource = this.Margins;
            MarginPicker.SelectedItem = this.DefaultMargin;

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
                _messageBus.Send(new Model.Messages.ChangesBrightness {
                    Brightness = (float)e.NewValue / 100
                });
            }
        }

        private void MarginPicker_SelectedIndexChanged(object sender, EventArgs e) {
            if (this.MarginPicker.SelectedIndex != -1) {
                var margin = int.Parse(this.Margins[this.MarginPicker.SelectedIndex]);
                this.SetMargin(margin);
                this.OnSet?.Invoke(this, e);
            }
        }

        private void FontPicker_SelectedIndexChanged(object sender, EventArgs e) {
            if (this.FontPicker.SelectedIndex != -1) {
                var fontSize = int.Parse(this.FontSizes[this.FontPicker.SelectedIndex]);
                this.SetFontSize(fontSize);
                this.OnSet?.Invoke(this, e);
            }
        }

        private void SetFontSize(int fontSize) {
            var json = new {
                FontSize = fontSize
            };

            _messages.Send("changeFontSize", json);
        }

        private void SetMargin(int margin) {
            var json = new {
                Margin = margin
            };

            _messages.Send("changeMargin", json);
        }
    }
}