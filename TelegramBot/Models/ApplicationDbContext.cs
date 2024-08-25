using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;

namespace TelegramBot.Models;

public class ApplicationDbContext:DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext>options):base(options)
    {
        
    }
    public DbSet<Service> Services { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<SubscriptionPeriod> SubscriptionPeriods { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Subscription>(entity =>
        {
            entity.HasKey(e => e.Id);
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.Id);
        });
        
        // Define the converter to serialize/deserialize the dictionary
        var dictionaryToJsonConverter = new ValueConverter<Dictionary<SubscriptionPeriod, decimal>, string>(
            v => JsonConvert.SerializeObject(v), // Convert dictionary to JSON string
            v => JsonConvert.DeserializeObject<Dictionary<SubscriptionPeriod, decimal>>(v)); // Convert JSON string back to dictionary

        // Apply the converter to the 'Pricing' property
        modelBuilder.Entity<Service>()
            .Property(e => e.Pricing)
            .HasConversion(dictionaryToJsonConverter);

        modelBuilder.Entity<Service>().HasData(
            new Service
            {
                Id = 1,
                Name = "Basic Service",
                Pricing = new Dictionary<SubscriptionPeriod, decimal>
                {
                    { new SubscriptionPeriod{Period = 30}, 9.99m }
                }
            },
            new Service
            {
                Id = 2,
                Name = "Premium Service",
                Pricing = new Dictionary<SubscriptionPeriod, decimal>
                {
                    {  new SubscriptionPeriod{Period = 30}, 19.99m },
                    { new SubscriptionPeriod{Period = 30}, 54.99m },
                    {  new SubscriptionPeriod{Period = 30}, 199.99m }
                }
            }
            
        );

        base.OnModelCreating(modelBuilder);
    }
    
}