namespace TelegramBot.Interfaces;

public interface IPaymentService
{
    Task<bool> ProcessPaymentAsync(int userId, decimal amount);
}