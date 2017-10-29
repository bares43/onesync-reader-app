using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using EbookReader.Service;
using Xamarin.Forms;

namespace EbookReader.Page.Reader.QuickPanelTab {
    public class Settings : StackLayout {

        List<string> FontSizes {
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

        List<string> Margins {
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

        private Picker fontPicker;
        private Picker marginPicker;

        private IWebViewMessages _messages;

        public event EventHandler OnSet;

        public Settings() {

            // IOC
            _messages = IocManager.Container.Resolve<IWebViewMessages>();

            Padding = new Thickness(10, 0);

            fontPicker = new Picker {
                ItemsSource = this.FontSizes,
                SelectedItem = this.DefaultFont,
                HorizontalOptions = LayoutOptions.EndAndExpand,
                VerticalOptions = LayoutOptions.Center,
                Title = Device.RuntimePlatform == Device.Android ? "Písmo" : "",
            };

            fontPicker.SelectedIndexChanged += FontPicker_SelectedIndexChanged;

            marginPicker = new Picker {
                ItemsSource = this.Margins,
                SelectedItem = this.DefaultMargin,
                HorizontalOptions = LayoutOptions.EndAndExpand,
                VerticalOptions = LayoutOptions.Center,
                Title = Device.RuntimePlatform == Device.Android ? "Odsazení" : "",
            };
            marginPicker.SelectedIndexChanged += MarginPicker_SelectedIndexChanged;

            if (Device.RuntimePlatform == Device.Android) {
                fontPicker.WidthRequest = 75;
                marginPicker.WidthRequest = 75;
            }

            var tableView = new TableView {
                Root = new TableRoot {
                    new TableSection("Vzhled") {
                        new ViewCell {
                            View = new StackLayout {
                                Orientation = StackOrientation.Horizontal,
                                VerticalOptions = LayoutOptions.Center,
                                Padding = new Thickness(10, 0),
                                Children = {
                                    new Label {
                                        Text = "Písmo",
                                        VerticalOptions = LayoutOptions.Center,
                                    },
                                    fontPicker
                                }
                            }
                        },
                        new ViewCell {
                            View = new StackLayout {
                                Orientation = StackOrientation.Horizontal,
                                VerticalOptions = LayoutOptions.Center,
                                Padding = new Thickness(10, 0),
                                Children = {
                                    new Label {
                                        Text = "Odsazení",
                                        VerticalOptions = LayoutOptions.Center,
                                    },
                                    marginPicker
                                }
                            }
                        }
                    }
                }
            };

            Device.BeginInvokeOnMainThread(() => {
                Children.Add(tableView);
            });

        }

        private void MarginPicker_SelectedIndexChanged(object sender, EventArgs e) {
            if (this.marginPicker.SelectedIndex != -1) {
                var margin = int.Parse(this.Margins[this.marginPicker.SelectedIndex]);
                this.SetMargin(margin);
                this.OnSet?.Invoke(this, e);
            }
        }

        private void FontPicker_SelectedIndexChanged(object sender, EventArgs e) {
            if (this.fontPicker.SelectedIndex != -1) {
                var fontSize = int.Parse(this.FontSizes[this.fontPicker.SelectedIndex]);
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
