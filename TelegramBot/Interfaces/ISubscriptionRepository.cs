using TelegramBot.Models;

namespace TelegramBot.Interfaces;

public interface ISubscriptionRepository
{
    Task<IEnumerable<Subscription>> GetUserSubscriptionsAsync(int userId);
    Task<Subscription> GetSubscriptionByIdAsync(int id);
    Task AddSubscriptionAsync(Subscription subscription);
    Task UpdateSubscriptionAsync(Subscription subscription);
}