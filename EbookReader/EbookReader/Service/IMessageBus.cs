using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbookReader.Service {
    public interface IMessageBus {
        void Send<T>(T message);
        void Subscribe<T>(Action<T> action, IEnumerable<string> tags = null);
        void UnSubscribe(string tag);
    }
}
