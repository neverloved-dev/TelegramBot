using System;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

class Program
{
    static void Main(string[] args)
    {
        var botService = new BotService();
        botService.Start();

        Console.WriteLine("Bot is running... Press Enter to exit.");
        Console.ReadLine();
    }
}
