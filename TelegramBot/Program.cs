using Telegram.Bot;
using TelegramBot;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Dynamic;

internal class Program
{
  
    async static Task Main()
    {
        var builder = new ConfigurationBuilder().AddJsonFile($"config.json", true, true);
        var config = builder.Build();
        string botToken = config["BotToken"];
        var botClient = new TelegramBotClient(botToken);
        var bot = new BotEngine(botClient);

        await bot.ListenForMessageAsync();
    }
}