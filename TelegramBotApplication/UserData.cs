using System.Collections.Generic;

public class UserData
{
    public List<SubscriptionService> Subscriptions { get; set; }

    public UserData()
    {
        Subscriptions = new List<SubscriptionService>();
    }
}
