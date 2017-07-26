using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EbookReader.DependencyService;
using EbookReader.Service;
using Newtonsoft.Json;
using Plugin.FilePicker;
using Xam.Plugin.Abstractions;
using Xamarin.Forms;

namespace EbookReader {
    public class App : Application {

        FormsWebView webView;
        Label info;

        public App() {

            webView = new FormsWebView() {
                ContentType = Xam.Plugin.Abstractions.Enumerations.WebViewContentType.StringData,
                WidthRequest = 500,
                HeightRequest = 500
            };

            info = new Label {
                WidthRequest = 500,
                HeightRequest = 50
            };

            var loadButton = new Button {
                WidthRequest = 500,
                HeightRequest = 50,
                Text = "Načíst knihu"
            };

            loadButton.Clicked += LoadButton_Clicked;

            this.LoadWebViewLayout();

            webView.OnContentLoaded += WebView_OnContentLoaded;

            webView.RegisterGlobalCallback("csDebug", (str) => {
                webView.InjectJavascript(string.Format("scroll({0})", str));
            });

            var content = new ContentPage {
                Title = "EbookReader",
                Content = new StackLayout {
                    VerticalOptions = LayoutOptions.Center,
                    Children = {
                        loadButton,
                        info,
                        webView
                    }
                }
            };

            MainPage = new NavigationPage(content);
        }

        private void LoadButton_Clicked(object sender, EventArgs e) {
            this.LoadBook();
        }

        public async void LoadBook() {
            var pickedFile = await CrossFilePicker.Current.PickFile();

            var loader = new EpubLoader();
            var epub = await loader.GetEpub(pickedFile.FileName, pickedFile.DataArray);

            info.Text = string.Format("Název: {0}, Autor: {1}\n Popis: {2}", epub.Title, epub.Author, epub.Description);
        }
                
        private string CreateData() {
            return @"
<p>Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Integer lacinia. Aenean vel massa quis mauris vehicula lacinia. Fusce tellus odio, dapibus id fermentum quis, suscipit id erat. Nulla est. Pellentesque arcu. Praesent in mauris eu tortor porttitor accumsan. Donec quis nibh at felis congue commodo. Mauris dolor felis, sagittis at, luctus sed, aliquam non, tellus. Duis viverra diam non justo. Maecenas ipsum velit, consectetuer eu lobortis ut, dictum at dui. Duis sapien nunc, commodo et, interdum suscipit, sollicitudin et, dolor. Nullam rhoncus aliquam metus. Nullam sapien sem, ornare ac, nonummy non, lobortis a enim. Proin in tellus sit amet nibh dignissim sagittis. Duis viverra diam non justo. Curabitur bibendum justo non orci. Suspendisse nisl.</p>

<p>Vivamus porttitor turpis ac leo. Maecenas sollicitudin. Aenean vel massa quis mauris vehicula lacinia. Maecenas fermentum, sem in pharetra pellentesque, velit turpis volutpat ante, in pharetra metus odio a lectus. Aenean fermentum risus id tortor. Nullam faucibus mi quis velit. Fusce tellus odio, dapibus id fermentum quis, suscipit id erat. Quis autem vel eum iure reprehenderit qui in ea voluptate velit esse quam nihil molestiae consequatur, vel illum qui dolorem eum fugiat quo voluptas nulla pariatur? Duis viverra diam non justo. Integer in sapien. Mauris suscipit, ligula sit amet pharetra semper, nibh ante cursus purus, vel sagittis velit mauris vel metus. Etiam dui sem, fermentum vitae, sagittis id, malesuada in, quam. Etiam sapien elit, consequat eget, tristique non, venenatis quis, ante. Mauris suscipit, ligula sit amet pharetra semper, nibh ante cursus purus, vel sagittis velit mauris vel metus. Nulla pulvinar eleifend sem. Quisque porta. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus.</p>

<p>Suspendisse nisl. Morbi scelerisque luctus velit. Nulla accumsan, elit sit amet varius semper, nulla mauris mollis quam, tempor suscipit diam nulla vel leo. In sem justo, commodo ut, suscipit at, pharetra vitae, orci. Aliquam erat volutpat. Aenean fermentum risus id tortor. Morbi imperdiet, mauris ac auctor dictum, nisl ligula egestas nulla, et sollicitudin sem purus in lacus. Proin mattis lacinia justo. Pellentesque pretium lectus id turpis. Nulla pulvinar eleifend sem. Pellentesque pretium lectus id turpis. Mauris suscipit, ligula sit amet pharetra semper, nibh ante cursus purus, vel sagittis velit mauris vel metus. Praesent id justo in neque elementum ultrices. Quisque tincidunt scelerisque libero. Sed vel lectus. Donec odio tempus molestie, porttitor ut, iaculis quis, sem. Pellentesque sapien. Etiam commodo dui eget wisi. Mauris suscipit, ligula sit amet pharetra semper, nibh ante cursus purus, vel sagittis velit mauris vel metus. Vivamus porttitor turpis ac leo. Fusce aliquam vestibulum ipsum.</p>

<p>Integer malesuada. Aenean vel massa quis mauris vehicula lacinia. Mauris dolor felis, sagittis at, luctus sed, aliquam non, tellus. Mauris tincidunt sem sed arcu. Proin pede metus, vulputate nec, fermentum fringilla, vehicula vitae, justo. Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Nulla turpis magna, cursus sit amet, suscipit a, interdum id, felis. Aliquam erat volutpat. Etiam egestas wisi a erat. Mauris dolor felis, sagittis at, luctus sed, aliquam non, tellus. Nullam faucibus mi quis velit. Donec iaculis gravida nulla. Fusce wisi. Nulla non arcu lacinia neque faucibus fringilla. In sem justo, commodo ut, suscipit at, pharetra vitae, orci. Nullam sapien sem, ornare ac, nonummy non, lobortis a enim.</p>

<p>In dapibus augue non sapien. Pellentesque sapien. Aenean vel massa quis mauris vehicula lacinia. In rutrum. Duis risus. Morbi imperdiet, mauris ac auctor dictum, nisl ligula egestas nulla, et sollicitudin sem purus in lacus. Nullam lectus justo, vulputate eget mollis sed, tempor sed magna. Integer vulputate sem a nibh rutrum consequat. Phasellus faucibus molestie nisl. Aenean fermentum risus id tortor. Duis condimentum augue id magna semper rutrum. Proin in tellus sit amet nibh dignissim sagittis. Fusce nibh. Praesent dapibus. Cras pede libero, dapibus nec, pretium sit amet, tempor quis.</p>

<p>Donec vitae arcu. Integer pellentesque quam vel velit. Curabitur bibendum justo non orci. Proin pede metus, vulputate nec, fermentum fringilla, vehicula vitae, justo. Aliquam ornare wisi eu metus. Proin pede metus, vulputate nec, fermentum fringilla, vehicula vitae, justo. Integer malesuada. Donec vitae arcu. Integer rutrum, orci vestibulum ullamcorper ultricies, lacus quam ultricies odio, vitae placerat pede sem sit amet enim. Nulla est. Integer malesuada. In laoreet, magna id viverra tincidunt, sem odio bibendum justo, vel imperdiet sapien wisi sed libero.</p>

<p>Nunc tincidunt ante vitae massa. Duis pulvinar. Aenean fermentum risus id tortor. Donec vitae arcu. In convallis. Duis sapien nunc, commodo et, interdum suscipit, sollicitudin et, dolor. Aliquam erat volutpat. Nulla turpis magna, cursus sit amet, suscipit a, interdum id, felis. Sed ac dolor sit amet purus malesuada congue. Nam sed tellus id magna elementum tincidunt. Duis ante orci, molestie vitae vehicula venenatis, tincidunt ac pede. Etiam quis quam.</p>

<p>Fusce tellus. In enim a arcu imperdiet malesuada. Nam sed tellus id magna elementum tincidunt. Neque porro quisquam est, qui dolorem ipsum quia dolor sit amet, consectetur, adipisci velit, sed quia non numquam eius modi tempora incidunt ut labore et dolore magnam aliquam quaerat voluptatem. Phasellus enim erat, vestibulum vel, aliquam a, posuere eu, velit. Integer lacinia. Pellentesque ipsum. Suspendisse nisl. Nam libero tempore, cum soluta nobis est eligendi optio cumque nihil impedit quo minus id quod maxime placeat facere possimus, omnis voluptas assumenda est, omnis dolor repellendus. Integer rutrum, orci vestibulum ullamcorper ultricies, lacus quam ultricies odio, vitae placerat pede sem sit amet enim. Maecenas aliquet accumsan leo. Sed ac dolor sit amet purus malesuada congue. Nam sed tellus id magna elementum tincidunt.</p>

<p>Mauris dictum facilisis augue. Duis viverra diam non justo. Aliquam erat volutpat. Cras pede libero, dapibus nec, pretium sit amet, tempor quis. Neque porro quisquam est, qui dolorem ipsum quia dolor sit amet, consectetur, adipisci velit, sed quia non numquam eius modi tempora incidunt ut labore et dolore magnam aliquam quaerat voluptatem. Nulla est. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Duis risus. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Quis autem vel eum iure reprehenderit qui in ea voluptate velit esse quam nihil molestiae consequatur, vel illum qui dolorem eum fugiat quo voluptas nulla pariatur? Integer tempor. Nullam faucibus mi quis velit. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Aenean placerat. Pellentesque ipsum. Aliquam erat volutpat. Nullam sapien sem, ornare ac, nonummy non, lobortis a enim. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum. Vestibulum fermentum tortor id mi.</p>

<p>Duis sapien nunc, commodo et, interdum suscipit, sollicitudin et, dolor. Nulla est. Nullam sit amet magna in magna gravida vehicula. Integer imperdiet lectus quis justo. Nullam rhoncus aliquam metus. Phasellus et lorem id felis nonummy placerat. Maecenas ipsum velit, consectetuer eu lobortis ut, dictum at dui. Mauris elementum mauris vitae tortor. Fusce tellus. Aliquam ornare wisi eu metus. Mauris suscipit, ligula sit amet pharetra semper, nibh ante cursus purus, vel sagittis velit mauris vel metus. Donec ipsum massa, ullamcorper in, auctor et, scelerisque sed, est.</p>
";   
        }

        private void SendHtml(string html) {
            var json = JsonConvert.SerializeObject(new {
                Html = html
            });

            var js = string.Format("loadHtml('{0}')", Base64Encode(json));

            webView.InjectJavascript(js);
        }

        public static string Base64Encode(string plainText) {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        private async void LoadWebViewLayout() {
            var fileContent = Xamarin.Forms.DependencyService.Get<IAssetsManager>();
            webView.Source = await fileContent.GetFileContentAsync("layout.html");
        }

        private void WebView_OnContentLoaded(Xam.Plugin.Abstractions.Events.Inbound.ContentLoadedDelegate eventObj) {
            eventObj.Sender.InjectJavascript("init()");
            this.SendHtml(this.CreateData());
        }

        protected override void OnStart() {
            // Handle when your app starts
        }

        protected override void OnSleep() {
            // Handle when your app sleeps
        }

        protected override void OnResume() {
            // Handle when your app resumes
        }
    }
}
