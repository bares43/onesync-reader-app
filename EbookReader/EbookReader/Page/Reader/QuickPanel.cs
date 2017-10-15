using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace EbookReader.Page.Reader {
    public class QuickPanel : StackLayout {

        StackLayout contentLayout;

        StackLayout tabSettings;
        StackLayout tabContents;

        Button buttonSettings;
        Button buttonContents;
        Button buttonClose;

        public QuickPanel() : base() {

            Orientation = StackOrientation.Vertical;
            IsVisible = false;
            BackgroundColor = Color.LightGray;

            buttonSettings = new Button {
                Text = "Nastavení",
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };
            buttonSettings.Clicked += ButtonSettings_Clicked;

            buttonContents = new Button {
                Text = "Obsah",
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };
            buttonContents.Clicked += ButtonContents_Clicked;

            buttonClose = new Button {
                Text = "Zavřít"
            };
            buttonClose.Clicked += ButtonClose_Clicked;

            var buttonsLayout = new StackLayout {
                Orientation = StackOrientation.Horizontal,
                Children = {
                    buttonSettings,
                    buttonContents,
                    buttonClose,
                }
            };

            contentLayout = new StackLayout {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };

            this.OpenSettings();

            Children.Add(buttonsLayout);
            Children.Add(contentLayout);
        }

        public void Show() {
            Device.BeginInvokeOnMainThread(() => {
                IsVisible = true;
            });
        }

        public void Hide() {
            Device.BeginInvokeOnMainThread(() => {
                IsVisible = false;
            });
        }

        private void ButtonClose_Clicked(object sender, EventArgs e) {
            this.Hide();
        }

        private void ButtonContents_Clicked(object sender, EventArgs e) {
            this.OpenContents();
        }

        private void ButtonSettings_Clicked(object sender, EventArgs e) {
            this.OpenSettings();
        }

        private void OpenTab(StackLayout tab) {
            Device.BeginInvokeOnMainThread(() => {
                contentLayout.Children.Clear();
                contentLayout.Children.Add(tab);
            });
        }

        private void OpenContents() {
            if (tabContents == null) {
                tabContents = this.CreateContentsTab();
            }

            this.OpenTab(tabContents);
        }

        private void OpenSettings() {
            if (tabSettings == null) {
                tabSettings = this.CreateSettingsTab();
            }

            this.OpenTab(tabSettings);
        }

        private StackLayout CreateSettingsTab() {
            var tab = new StackLayout {
                Children = {
                    new Label {
                        Text = "rychlé nastavení"
                    }
                }
            };

            return tab;
        }

        private StackLayout CreateContentsTab() {
            var tab = new StackLayout {
                Children = {
                    new Label {
                        Text = "obsah knihy"
                    }
                }
            };

            return tab;
        }

    }
}
