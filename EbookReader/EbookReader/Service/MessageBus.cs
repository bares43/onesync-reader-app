using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbookReader.Service {
    public class MessageBus : IMessageBus {

        private readonly Dictionary<Type, List<Subscriber>> _handlers = new Dictionary<Type, List<Subscriber>>();

        public void Send<T>(T message) {
            var type = typeof(T);

            var handlers = _handlers.Where(o => o.Key == type).SelectMany(o => o.Value).ToList();

            foreach (var handler in handlers) {
                if (handler != null && handler.Delegate is Action<T> action) {
                    action.Invoke(message);
                }
            }
        }

        public void Subscribe<T>(Action<T> action, IEnumerable<string> tags = null) {
            var messageType = typeof(T);

            var subscriber = new Subscriber {
                Delegate = action,
                Tags = tags
            };

            if (_handlers.ContainsKey(messageType)) {
                _handlers[messageType].Add(subscriber);
            } else {
                _handlers[messageType] = new List<Subscriber> { subscriber };
            }
        }

        public void UnSubscribe(string tag) {
           foreach(var action in _handlers.Values) {
                action.RemoveAll(o => o.Tags.Contains(tag));
            }
        }

        private class Subscriber {

            private IEnumerable<string> _tags;

            public Delegate Delegate { get; set; }
            public IEnumerable<string> Tags {
                get {
                    if (_tags == null) return new string[0];

                    return _tags;
                }
                set {
                    _tags = value;
                }
            }
        }
    }
}