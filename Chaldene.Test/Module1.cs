using Chaldene.Data.Messages;
using Chaldene.Data.Messages.Receivers;
using Chaldene.Modules;
using Chaldene.Utils.Scaffolds;

namespace Chaldene.Test
{
    public class Module1 : IModule
    {
        public async void Execute(MessageReceiverBase @base)
        {
            var receiver = @base.Concretize<GroupMessageReceiver>();
            var plain = receiver.MessageChain.GetPlainMessage();

        }

        public bool? IsEnable { get; set; }
    }
}