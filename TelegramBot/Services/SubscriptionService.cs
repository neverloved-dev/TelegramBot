using TelegramBot.Interfaces;
using TelegramBot.Models;

namespace TelegramBot.Services;

public class SubscriptionService : ISubscriptionService
{
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IServiceRepository _serviceRepository;
    private readonly IPaymentService _paymentService;

    public SubscriptionService(
        ISubscriptionRepository subscriptionRepository,
        IServiceRepository serviceRepository,
        IPaymentService paymentService)
    {
        _subscriptionRepository = subscriptionRepository;
        _serviceRepository = serviceRepository;
        _paymentService = paymentService;
    }

    public async Task<Subscription> CreateSubscriptionAsync(int userId, int serviceId, SubscriptionPeriod period)
    {
        var service = await _serviceRepository.GetServiceByIdAsync(serviceId);
        if (service == null) throw new ArgumentException("Invalid service ID.");

        var price = service.Pricing[period];
        var paymentResult = await _paymentService.ProcessPaymentAsync(userId, price);
        if (!paymentResult) throw new InvalidOperationException("Payment failed.");

        var subscription = new Subscription
        {
            ServiceId = serviceId,
            UserId = userId,
            Period = period,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(period.Period),
            Status = SubscriptionStatus.Active
        };

        await _subscriptionRepository.AddSubscriptionAsync(subscription);
        return subscription;
    }

    public async Task<Subscription> ChangeSubscriptionAsync(int subscriptionId, SubscriptionPeriod newPeriod)
    {
        var subscription = await _subscriptionRepository.GetSubscriptionByIdAsync(subscriptionId);
        if (subscription == null)
        {
            throw new KeyNotFoundException($"Subscription with ID {subscriptionId} not found.");
        }

        var service = await _serviceRepository.GetServiceByIdAsync(subscription.ServiceId);
        if (service == null)
        {
            throw new KeyNotFoundException($"Service with ID {subscription.ServiceId} not found.");
        }

        if (!service.Pricing.TryGetValue(newPeriod, out var newPrice))
        {
            throw new KeyNotFoundException($"Pricing for the period '{newPeriod.Period}' not found in service '{service.Name}'.");
        }

        // Process payment and other business logic
        bool paymentSucceeded = await _paymentService.ProcessPaymentAsync(subscription.UserId, newPrice);
        if (!paymentSucceeded)
        {
            throw new InvalidOperationException("Payment processing failed.");
        }

        // Update subscription details...
        subscription.Period = newPeriod;
        subscription.EndDate = DateTime.UtcNow.AddDays(newPeriod.Period);
        await _subscriptionRepository.UpdateSubscriptionAsync(subscription);

        return subscription;
    }


    public async Task CancelSubscriptionAsync(int subscriptionId)
    {
        var subscription = await _subscriptionRepository.GetSubscriptionByIdAsync(subscriptionId);
        if (subscription == null) throw new ArgumentException("Invalid subscription ID.");

        subscription.Status = SubscriptionStatus.Canceled;
        await _subscriptionRepository.UpdateSubscriptionAsync(subscription);
    }

    public async Task<IEnumerable<Subscription>> GetUserSubscriptionsAsync(int userId)
    {
        return await _subscriptionRepository.GetUserSubscriptionsAsync(userId);
    }
}
