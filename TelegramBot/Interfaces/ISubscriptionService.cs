using TelegramBot.Models;

namespace TelegramBot.Interfaces;

public interface ISubscriptionService
{
    Task<Subscription> CreateSubscriptionAsync(int userId, int serviceId, SubscriptionPeriod period);
    Task<Subscription> ChangeSubscriptionAsync(int subscriptionId, SubscriptionPeriod newPeriod);
    Task CancelSubscriptionAsync(int subscriptionId);
    Task<IEnumerable<Subscription>> GetUserSubscriptionsAsync(int userId);
}