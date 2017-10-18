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

        public event EventHandler<int> OnFontChange;
        public event EventHandler<int> OnMarginChange;

        private Picker fontPicker;
        private Picker marginPicker;

        private IWebViewMessages _messages;

        public Settings() {

            // IOC
            _messages = IocManager.Container.Resolve<IWebViewMessages>();

            Padding = new Thickness(10, 0);

            fontPicker = new Picker {
                ItemsSource = this.FontSizes,
                SelectedItem = "20",
                HorizontalOptions = LayoutOptions.EndAndExpand,
            };

            fontPicker.SelectedIndexChanged += FontPicker_SelectedIndexChanged;

            marginPicker = new Picker {
                ItemsSource = this.Margins,
                SelectedItem = "30",
                HorizontalOptions = LayoutOptions.EndAndExpand,
            };

            marginPicker.SelectedIndexChanged += MarginPicker_SelectedIndexChanged;

            var tableView = new TableView {
                Root = new TableRoot {
                    new TableSection("Vzhled") {
                        new ViewCell {
                            View = new StackLayout {
                                Orientation = StackOrientation.Horizontal,
                                Padding = new Thickness(10),
                                Children = {
                                    new Label {
                                        Text = "Písmo"
                                    },
                                    fontPicker
                                }
                            }
                        },
                        new ViewCell {
                            View = new StackLayout {
                                Orientation = StackOrientation.Horizontal,
                                Padding = new Thickness(10),
                                Children = {
                                    new Label {
                                        Text = "Odsazení"
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
                //this.OnMarginChange?.Invoke(this, margin);
                this.SetMargin(margin);
            }
        }

        private void FontPicker_SelectedIndexChanged(object sender, EventArgs e) {
            if (this.fontPicker.SelectedIndex != -1) {
                var fontSize = int.Parse(this.FontSizes[this.fontPicker.SelectedIndex]);
                //this.OnFontChange?.Invoke(this, fontSize);
                this.SetFontSize(fontSize);
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
