using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Models;

public class MenuHandler
{
    private readonly ITelegramBotClient _botClient;
    private readonly Dictionary<long, UserData> _usersData;
    private readonly HttpClient _httpClient;

    public MenuHandler(ITelegramBotClient botClient)
    {
        _botClient = botClient;
        _usersData = new Dictionary<long, UserData>();
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri("https://yourapiaddress.com"); // Укажите ваш URL API
    }

    public async Task HandleMessageAsync(Message message)
    {
        var chatId = message.Chat.Id;
        var userMessage = message.Text;

        if (!_usersData.ContainsKey(chatId))
        {
            _usersData[chatId] = new UserData();
        }

        var userData = _usersData[chatId];

        switch (userMessage)
        {
            case "/start":
                await ShowMainMenu(chatId);
                break;
            case "Сервисы":
                await ShowServicesMenu(chatId);
                break;
            case "Личный кабинет":
                await ShowProfileMenu(chatId);
                break;
            case "Назад":
                await ShowMainMenu(chatId);
                break;
            default:
                await _botClient.SendTextMessageAsync(chatId, "Команда не распознана. Пожалуйста, выберите пункт меню.");
                break;
        }
    }

    private async Task ShowMainMenu(long chatId)
    {
        var replyKeyboardMarkup = new ReplyKeyboardMarkup(new[]
        {
            new KeyboardButton[] { "Сервисы", "Личный кабинет" }
        })
        {
            ResizeKeyboard = true
        };

        await _botClient.SendTextMessageAsync(
            chatId,
            "Главное меню",
            replyMarkup: replyKeyboardMarkup
        );
    }

    private async Task ShowServicesMenu(long chatId)
    {
        var services = await GetServicesAsync();
        if (services == null)
        {
            await _botClient.SendTextMessageAsync(chatId, "Не удалось получить список сервисов.");
            return;
        }

        var buttons = new List<KeyboardButton[]>();
        foreach (var service in services)
        {
            buttons.Add(new[] { new KeyboardButton(service.Name) });
        }
        buttons.Add(new[] { new KeyboardButton("Назад") });

        var replyKeyboardMarkup = new ReplyKeyboardMarkup(buttons)
        {
            ResizeKeyboard = true
        };

        await _botClient.SendTextMessageAsync(
            chatId,
            "Меню Сервисы: выберите сервис для подписки.",
            replyMarkup: replyKeyboardMarkup
        );
    }

    private async Task ShowProfileMenu(long chatId)
    {
        var subscriptions = await GetUserSubscriptionsAsync(chatId);
        if (subscriptions == null)
        {
            await _botClient.SendTextMessageAsync(chatId, "Не удалось получить информацию о подписках.");
            return;
        }

        var messageText = "Ваши подписки:\n";
        foreach (var subscription in subscriptions)
        {
            messageText += $"- {subscription.ServiceName}: {subscription.Status}, осталось дней: {subscription.DaysRemaining}\n";
        }

        var replyKeyboardMarkup = new ReplyKeyboardMarkup(new[]
        {
            new KeyboardButton[] { "Назад" }
        })
        {
            ResizeKeyboard = true
        };

        await _botClient.SendTextMessageAsync(
            chatId,
            messageText,
            replyMarkup: replyKeyboardMarkup
        );
    }

    private async Task<List<Service>> GetServicesAsync()
    {
        var response = await _httpClient.GetAsync("/api/services");
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Service>>(json);
        }

        return null;
    }

    private async Task<List<SubscriptionService>> GetUserSubscriptionsAsync(long chatId)
    {
        var response = await _httpClient.GetAsync($"/api/subscriptions/{chatId}");
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<SubscriptionService>>(json);
        }

        return null;
    }
}
