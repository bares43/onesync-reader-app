using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbookReader.Model.Messages {
    public class OAuth2AccessTokenObtainedMessage {
        public string Provider { get; set; }
        public string AccessToken { get; set; }
    }
}
