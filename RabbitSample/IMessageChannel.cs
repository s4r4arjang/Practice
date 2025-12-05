global using ChannelX = System.Threading.Channels.Channel;
using System.Threading.Channels;

namespace RabbitSample
{
    public interface IMessageChannel
    {
        Channel<string> Channel { get; }
    }

    public class MessageChannel : IMessageChannel
    {
        public Channel<string> Channel { get; }

        public MessageChannel ()
        {
            Channel=ChannelX.CreateUnbounded<string>();
        }
    }
}
