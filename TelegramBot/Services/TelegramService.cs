using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot.Interfaces;

public class TelegramBotService
{
    private readonly ITelegramBotClient _botClient;
    private readonly ISubscriptionService _subscriptionService;
    private readonly IServiceRepository _serviceRepository;

    public TelegramBotService(ITelegramBotClient botClient, 
        ISubscriptionService subscriptionService,
        IServiceRepository serviceRepository)
    {
        _botClient = botClient;
        _subscriptionService = subscriptionService;
        _serviceRepository = serviceRepository;
    }

    public async Task HandleUpdateAsync(Update update)
    {
        if (update.Type == UpdateType.Message && update.Message.Type == MessageType.Text)
        {
            var message = update.Message;
            var userId = message.From.Id;
            var text = message.Text;

            switch (text.ToLower())
            {
                case "/start":
                    await _botClient.SendTextMessageAsync(userId, "Welcome to the Subscription Manager!");
                    break;

                case "/services":
                    await HandleServicesCommand(userId);
                    break;

                case "/subscriptions":
                    await HandleSubscriptionsCommand(userId);
                    break;

                default:
                    await _botClient.SendTextMessageAsync(userId, "Unknown command. Please use /services or /subscriptions.");
                    break;
            }
        }
    }

    private async Task HandleServicesCommand(long userId)
    {
        var services = await _serviceRepository.GetAllServicesAsync();
        string response = "Available services:\n\n";

        foreach (var service in services)
        {
            response += $"{service.Name} - {service.Description}\n";
            foreach (var pricing in service.Pricing)
            {
                response += $"  {pricing.Key}: {pricing.Value} USD\n";
            }
            response += "\n";
        }

        await _botClient.SendTextMessageAsync(userId, response);
    }

    private async Task HandleSubscriptionsCommand(long userId)
    {
        var subscriptions = await _subscriptionService.GetUserSubscriptionsAsync((int)userId);
        string response = "Your subscriptions:\n\n";

        if (!subscriptions.Any())
        {
            response = "You have no active subscriptions.";
        }
        else
        {
            foreach (var subscription in subscriptions)
            {
                response += $"Service: {subscription.Service.Name}\n" +
                            $"Period: {subscription.Period}\n" +
                            $"Status: {subscription.Status}\n" +
                            $"End Date: {subscription.EndDate}\n\n";
            }
        }

        await _botClient.SendTextMessageAsync(userId, response);
    }
}
