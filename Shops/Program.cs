using System;
using Shops.UI;
using Spectre.Console.Cli;
namespace Shops
{
    internal class Program
    {
        private static void Main()
        {
            CommandManager manager = new CommandManager();
            while (manager.Running)
            {
                manager.Execute();
            }
        }
    }
}
