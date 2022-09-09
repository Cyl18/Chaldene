using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Manganese.Array;
using Manganese.Text;
using Chaldene.Data.Events;
using Chaldene.Data.Messages;
using Chaldene.Data.Messages.Concretes;
using Chaldene.Data.Messages.Receivers;
using Chaldene.Data.Shared;
using Chaldene.Sessions;
using Chaldene.Utils.Scaffolds;
using File = System.IO.File;

namespace Chaldene.Test
{
    internal static class Program
    {
        private static async Task Main()
        {
            await PrivateTest.Run();
        }
    }
}