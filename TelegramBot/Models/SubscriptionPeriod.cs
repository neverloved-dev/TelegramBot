namespace TelegramBot.Models;

public class SubscriptionPeriod
{
    public int Period { get; set; }
    
    public override bool Equals(object obj)
    {
        if (obj is SubscriptionPeriod other)
        {
            return this.Period == other.Period;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return Period.GetHashCode();
    }
}