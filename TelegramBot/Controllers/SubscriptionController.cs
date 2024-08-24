using Microsoft.AspNetCore.Mvc;
using TelegramBot.Interfaces;
using TelegramBot.Models;

[ApiController]
[Route("api/subscriptions")]
public class SubscriptionController : ControllerBase
{
    private readonly ISubscriptionService _subscriptionService;

    public SubscriptionController(ISubscriptionService subscriptionService)
    {
        _subscriptionService = subscriptionService;
    }

    [HttpPost("subscribe")]
    public async Task<IActionResult> Subscribe(int userId, int serviceId, SubscriptionPeriod period)
    {
        var subscription = await _subscriptionService.CreateSubscriptionAsync(userId, serviceId, period);
        return Ok(subscription);
    }

    [HttpPost("change")]
    public async Task<IActionResult> ChangeSubscription(int subscriptionId, SubscriptionPeriod newPeriod)
    {
        var subscription = await _subscriptionService.ChangeSubscriptionAsync(subscriptionId, newPeriod);
        return Ok(subscription);
    }

    [HttpPost("cancel")]
    public async Task<IActionResult> CancelSubscription(int subscriptionId)
    {
        await _subscriptionService.CancelSubscriptionAsync(subscriptionId);
        return Ok("Subscription canceled.");
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserSubscriptions(int userId)
    {
        var subscriptions = await _subscriptionService.GetUserSubscriptionsAsync(userId);
        return Ok(subscriptions);
    }
}