using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EbookReader.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xam.Plugin.Abstractions;

namespace EbookReader.Service {
    public class WebViewMessages {

        FormsWebView _webView;

        public event EventHandler<Model.WebViewMessages.MyMessage> OnMyMessage;

        public WebViewMessages(FormsWebView webView) {
            _webView = webView;

            webView.RegisterGlobalCallback("csCallback", (data) => {
                this.Parse(data);
            });
        }

        public void Send(string action, object data) {
            var json = JsonConvert.SerializeObject(new {
                Action = action,
                Data = data,
            });

            var toSend = Base64Helper.Encode(json);
            _webView.InjectJavascript(string.Format("Messages.parse('{0}')", toSend));
        }

        public void Parse(string data) {
            var json = JsonConvert.DeserializeObject<Model.WebViewMessages.Message>(Base64Helper.Decode(data));

            var messageType = Type.GetType(string.Format("EbookReader.Model.WebViewMessages.{0}", json.Action));
            var msg = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(json.Data), messageType);

            switch (json.Action) {
                case Model.WebViewMessages.MyMessage.Name:
                    this.OnMyMessage?.Invoke(this, msg as Model.WebViewMessages.MyMessage);
                    break;
            }

        }
    }
}
