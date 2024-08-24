using TelegramBot.Interfaces;

namespace TelegramBot.Services;

public class PaymentService:IPaymentService
{
    public Task<bool> ProcessPaymentAsync(int userId, decimal amount)
    {
        return Task.FromResult(true);
    }
}