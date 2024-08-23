namespace TelegramBot.Models;

public class Subscription
{
    public int Id { get; set; }
    public int ServiceId { get; set; }
    public SubscriptionPeriod Period { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public SubscriptionStatus Status { get; set; }
}