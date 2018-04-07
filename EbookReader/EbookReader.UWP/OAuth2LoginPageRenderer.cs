using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using EbookReader.Model;
using EbookReader.Model.Messages;
using EbookReader.Page;
using EbookReader.Service;
using EbookReader.UWP;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Xamarin.Auth;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(OAuth2LoginPage), typeof(OAuth2LoginPageRenderer))]
namespace EbookReader.UWP {
    public class OAuth2LoginPageRenderer : PageRenderer {
        private Frame _frame;
        private bool _isShown;

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Page> e) {
            base.OnElementChanged(e);

            if (_isShown) return;
            _isShown = true;

            if (Control == null) {
                WindowsPage windowsPage = new WindowsPage();

                var OAuth2Data = Xamarin.Forms.Application.Current.Properties["OAuth2Data"] as OAuth2RequestData;

                var auth = new OAuth2Authenticator(
                    OAuth2Data.ClientID,
                    OAuth2Data.Scope,
                    new Uri(OAuth2Data.AuthorizeUrl),
                    new Uri(OAuth2Data.RedirectUrl)
                ) {
                    ClearCookiesBeforeLogin = true
                };

                _frame = windowsPage.Frame;
                if (_frame == null) {
                    _frame = new Frame();
                    windowsPage.Content = _frame;
                    SetNativeControl(windowsPage);
                }

                auth.Completed += (sender, arg) => {
                    if (arg.IsAuthenticated) {
                        IocManager.Container.Resolve<IMessageBus>().Send(new OAuth2AccessTokenObtainedMessage {
                            AccessToken = arg.Account.Properties["access_token"],
                            Provider = OAuth2Data.Provider,
                        });
                    }

                    IocManager.Container.Resolve<IMessageBus>().Send(new OAuth2LoginPageClosed {
                        Provider = OAuth2Data.Provider,
                    });
                };

                auth.Error += (sender, arg) => {
                    IocManager.Container.Resolve<IMessageBus>().Send(new OAuth2LoginPageClosed {
                        Provider = OAuth2Data.Provider,
                    });
                };

                var pageType = auth.GetUI();
                _frame.Navigate(pageType, auth);
                Window.Current.Activate();
            }
        }
    }
}
