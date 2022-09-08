using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Manganese.Array;
using Manganese.Text;
using Mirai.Net.Data.Events;
using Mirai.Net.Data.Messages;
using Mirai.Net.Data.Messages.Concretes;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Sessions;
using Mirai.Net.Utils.Scaffolds;

namespace Mirai.Net.Test
{
    internal static class Program
    {
        private static async Task Main()
        {
            var exit = new ManualResetEvent(false);
            
            var bot = new MiraiBot("localhost:8080", "1145141919810", "1590454991");
            await bot.LaunchAsync();

            bot.GroupMessageReceived += async (sender, args) =>
            {
                if (args.MessageChain.GetPlainMessage() == "Hello, World!")
                {
                    await bot.SendGroupMessageAsync(args.GroupId, "hi!");
                }
            };
            
            Console.WriteLine("launched");
            exit.WaitOne();
        }
    }
}