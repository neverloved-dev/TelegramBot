using System;

public class SubscriptionService
{
    public string ServiceName { get; set; }
    public string Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime NextPaymentDate { get; set; }
    public string Status { get; set; }
    public string BotLink { get; set; }
    public int DaysRemaining { get; set; } // Новое свойство для отображения оставшихся дней
}
