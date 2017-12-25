using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using EbookReader.Model.Messages;
using EbookReader.Service;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EbookReader.Page.Reader {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QuickPanel : StackLayout {

        IMessageBus _messageBus;

        public QuickPanel() {

            _messageBus = IocManager.Container.Resolve<IMessageBus>();

            InitializeComponent();

            this.OpenSettings();

            _messageBus.Subscribe<CloseQuickPanel>((msg) => this.Hide());

        }
        
        private void PanelContent_OnChapterChange(object sender, Model.Navigation.Item e) {
            this.Hide();
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

        private void ButtonBookmarks_Clicked(object sender, EventArgs e) {
            this.OpenBookmarks();
        }

        private void OpenContents() {
            PanelContent.IsVisible = true;
            PanelSettings.IsVisible = false;
            PanelBookmarks.IsVisible = false;
        }

        private void OpenSettings() {
            PanelSettings.IsVisible = true;
            PanelContent.IsVisible = false;
            PanelBookmarks.IsVisible = false;
        }

        private void OpenBookmarks() {
            PanelBookmarks.IsVisible = true;
            PanelSettings.IsVisible = false;
            PanelContent.IsVisible = false;
        }
    }
}