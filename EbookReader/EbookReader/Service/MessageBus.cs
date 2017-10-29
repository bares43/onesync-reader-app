using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbookReader.Service {
    public class MessageBus : IMessageBus {

        private readonly Dictionary<Type, List<Delegate>> _handlers = new Dictionary<Type, List<Delegate>>();

        public void Send<T>(T message) {
            var type = typeof(T);

            foreach (var handler in _handlers.Where(o => o.Key == type).SelectMany(o => o.Value)) {
                if (handler is Action<T> action) {
                    action.Invoke(message);
                }
            }
        }

        public void Subscribe<T>(Action<T> action) {
            var messageType = typeof(T);

            if (_handlers.ContainsKey(messageType)) {
                _handlers[messageType].Add(action);
            } else {
                _handlers[messageType] = new List<Delegate> { action };
            }
        }
    }
}