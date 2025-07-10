using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot
{
    public class BotEngine
    {
        private readonly TelegramBotClient _botClient;
        private string[] MotivationMessages;
        private string[] CatsPictures;
        private string[] DogsPictures;


        public BotEngine(TelegramBotClient botClient)
        {
            _botClient = botClient;
            MotivationMessages = InitializeMotivationMessages();
            CatsPictures = InitializeImages("cats");
            DogsPictures = InitializeImages("dogs");
        }

        private string[] InitializeMotivationMessages()
        {
            string path = Environment.CurrentDirectory + "/Content/Texts/Motivation.txt";
            try
            {
                string[] result = System.IO.File.ReadAllLines(path);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"При инициализации мотивационных сообщений произошла ошибка: {ex.ToString()}");
                return Array.Empty<string>();
            }
        }

        private string[] InitializeImages(string folder)
        {
            string path = Environment.CurrentDirectory + "\\Content\\Pictures\\" + folder;
            try
            {
                string[] result = Directory.GetFiles(path);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"При инициализации изображений произошла ошибка: {ex.ToString()}");
                return Array.Empty<string>();
            }
        }


        public async Task ListenForMessageAsync()
        {
            using var cts = new CancellationTokenSource();

            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = Array.Empty<UpdateType>()
            };

            _botClient.StartReceiving(
                updateHandler: HandleUpdateAsync,
                pollingErrorHandler: HandlePollingErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: cts.Token
            );

            var me = await _botClient.GetMeAsync();
            Console.ReadLine();

        }

        private async Task HandleUpdateAsync(ITelegramBotClient client, Update update, CancellationToken token)
        {
            if (update.Message is not { } message)
                return;

            if (message.Text is not { } messageText)
                return;

            Console.WriteLine($"Received a {messageText} message in char {message.Chat.Id}");

            (int, int) randomNumbers;
            switch (message.Text) 
            {
                case "/cats":
                case "Котик 🐈":
                    randomNumbers = HelperMethods.GetRandomNubmers(MotivationMessages.Length, CatsPictures.Length);
                    await SendPhotoMessageAsync(client, message, MotivationMessages[randomNumbers.Item1], CatsPictures[randomNumbers.Item2], token);
                    break;
                case "/dogs":
                case "Собачка 🐶":
                    randomNumbers = HelperMethods.GetRandomNubmers(MotivationMessages.Length, CatsPictures.Length);
                    await SendPhotoMessageAsync(client, message, MotivationMessages[randomNumbers.Item1], DogsPictures[randomNumbers.Item2], token);
                    break;
                case "/help":
                case "Помощь 🛠️":
                    string text = $"У бота есть следующие команды:\n/cats\n/dogs";
                    await SendTextMessageAsync(client, message, text, token);
                    break;
                default:
                    await SendTextMessageAsync(client, message, "Такой команды не существует, Вызовите /help для просмотра команд", token);
                    break;



            }
        }

        private static ReplyKeyboardMarkup GetButtonKeyboard()
        {
            var keyboard = new ReplyKeyboardMarkup(new KeyboardButton[][]
            {
                new []
                {
                    new KeyboardButton("Котик 🐈")
                },
                new []
                {
                    new KeyboardButton("Собачка 🐶")
                },
                new []
                {
                    new KeyboardButton("Помощь 🛠️")
                }
            });
            return keyboard;
        }

        private static async Task SendTextMessageAsync(ITelegramBotClient botClient, Message message, string text, CancellationToken cancellationToken)
        {
            Message sendMessage = await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: text,
                replyMarkup: GetButtonKeyboard()
            );
        }

        private static async Task SendPhotoMessageAsync(ITelegramBotClient botClient, Message message, string motivation, string picture, CancellationToken cancellationToken)
        {
            using (var fileStream = new FileStream(picture, FileMode.Open, FileAccess.Read))
            { 
                Message sendMessage = await botClient.SendPhotoAsync(
                    chatId: message.Chat.Id,
                    photo: InputFile.FromStream(fileStream),
                    caption: motivation,
                    parseMode: ParseMode.Html,
                    cancellationToken: cancellationToken
                );
            }
        }

      
        private Task HandlePollingErrorAsync(ITelegramBotClient client, Exception exception, CancellationToken token)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                 => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }

    }
}
