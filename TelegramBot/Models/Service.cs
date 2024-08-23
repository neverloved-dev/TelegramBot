namespace TelegramBot.Models;

public class Service
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public Dictionary<SubscriptionPeriod, decimal> Pricing { get; set; } 
}
