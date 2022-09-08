using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Manganese.Array;
using Manganese.Text;
using Mirai.Net.Data.Messages.Concretes;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Sessions;

namespace Mirai.Net.Test
{
    internal static class Program
    {
        private static async Task Main()
        {
            var exit = new ManualResetEvent(false);
            
            var bot = new MiraiBot
            {
                Address = "localhost:8080",
                VerifyKey = "1145141919810",
                QQ = "1590454991"
            };
            
            await bot.LaunchAsync().ConfigureAwait(false);
            
            bot.MessageReceived
                .OfType<GroupMessageReceiver>()
                .Subscribe(async r =>
                {
                    var ums = r.MessageChain.OfType<UnknownMessage>();
                    if (ums.Any())
                    {
                        ums.Output(x => x.ToJsonString());
                    }
                });
            
            

            Console.WriteLine("launched");
            exit.WaitOne();
        }
    }
}