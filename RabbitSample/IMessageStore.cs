using System.Collections.Concurrent;

namespace RabbitSample
{
    public interface IMessageStore
    {
        void Add(string json);
        bool TryGet(out string json);
    }

    public class MessageStore : IMessageStore
    {
        private readonly ConcurrentQueue<string> _queue = new();

        public void Add(string json)
        {
            _queue.Enqueue(json);
        }

        public bool TryGet(out string json)
        {
            return _queue.TryDequeue(out json);
        }
    }
}
