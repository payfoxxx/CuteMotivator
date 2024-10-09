using Telegram.Bot;
using TelegramBot;

internal class Program
{
  
    async static Task Main()
    {
        string botToken = "<YOUR TOKEN>";
        var botClient = new TelegramBotClient(botToken);
        var bot = new BotEngine(botClient);

        await bot.ListenForMessageAsync();
    }

 
}