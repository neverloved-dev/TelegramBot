using System;
using System.Threading.Tasks;
using Moq;
using TelegramBot.Interfaces;
using TelegramBot.Models;
using TelegramBot.Services;
using Xunit;

public class SubscriptionServiceTests
{
    private readonly Mock<ISubscriptionRepository> _subscriptionRepositoryMock;
    private readonly Mock<IServiceRepository> _serviceRepositoryMock;
    private readonly Mock<IPaymentService> _paymentServiceMock;
    private readonly SubscriptionService _subscriptionService;

    public SubscriptionServiceTests()
    {
        _subscriptionRepositoryMock = new Mock<ISubscriptionRepository>();
        _serviceRepositoryMock = new Mock<IServiceRepository>();
        _paymentServiceMock = new Mock<IPaymentService>();

        _subscriptionService = new SubscriptionService(
            _subscriptionRepositoryMock.Object,
            _serviceRepositoryMock.Object,
            _paymentServiceMock.Object);
    }

    [Fact]
    public async Task CreateSubscriptionAsync_ShouldCreateSubscription_WhenPaymentSucceeds()
    {
        // Arrange
        var userId = 1;
        var serviceId = 1;
        var subscriptionPeriod = new SubscriptionPeriod { Period = 30 }; // Assuming 30 days period for 1 month
        var service = new Service
        {
            Id = serviceId,
            Name = "Test Service",
            Pricing = new Dictionary<SubscriptionPeriod, decimal>
            {
                { new SubscriptionPeriod { Period = subscriptionPeriod.Period }, 10.0m }
            }
        };

        _serviceRepositoryMock.Setup(repo => repo.GetServiceByIdAsync(serviceId))
            .ReturnsAsync(service);

        _paymentServiceMock.Setup(service => service.ProcessPaymentAsync(userId, 10.0m))
            .ReturnsAsync(true);

        // Act
        var result = await _subscriptionService.CreateSubscriptionAsync(userId, serviceId, subscriptionPeriod);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(serviceId, result.ServiceId);
        Assert.Equal(userId, result.UserId);
        Assert.Equal(subscriptionPeriod.Period, result.Period.Period);
        _subscriptionRepositoryMock.Verify(repo => repo.AddSubscriptionAsync(It.IsAny<Subscription>()), Times.Once);
    }

    [Fact]
    public async Task CreateSubscriptionAsync_ShouldThrowException_WhenPaymentFails()
    {
        // Arrange
        var userId = 1;
        var serviceId = 1;
        var subscriptionPeriod = new SubscriptionPeriod { Period = 30 }; // Assuming 30 days period for 1 month
        var service = new Service
        {
            Id = serviceId,
            Name = "Test Service",
            Pricing = new Dictionary<SubscriptionPeriod, decimal>
            {
                { new SubscriptionPeriod { Period = subscriptionPeriod.Period }, 10.0m }
            }
        };

        _serviceRepositoryMock.Setup(repo => repo.GetServiceByIdAsync(serviceId))
            .ReturnsAsync(service);

        _paymentServiceMock.Setup(service => service.ProcessPaymentAsync(userId, 10.0m))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _subscriptionService.CreateSubscriptionAsync(userId, serviceId, subscriptionPeriod));

        _subscriptionRepositoryMock.Verify(repo => repo.AddSubscriptionAsync(It.IsAny<Subscription>()), Times.Never);
    }
    
    [Fact]
    public async Task ChangeSubscriptionAsync_ShouldUpdateSubscription_WhenPaymentSucceeds()
    {
        // Arrange
        var subscriptionId = 1;
        var newPeriod = new SubscriptionPeriod { Period = 365 }; // Assuming 365 days period for 1 year
        var subscription = new Subscription
        {
            Id = subscriptionId,
            ServiceId = 1,
            UserId = 1,
            Period = new SubscriptionPeriod { Period = 30 }, // Original period is 30 days (1 month)
            EndDate = DateTime.UtcNow.AddMonths(1),
            Status = SubscriptionStatus.Active
        };
        var service = new Service
        {
            Id = subscription.ServiceId,
            Name = "Test Service",
            Pricing = new Dictionary<SubscriptionPeriod, decimal>
            {
                { new SubscriptionPeriod { Period = 365 }, 100.0m } // New pricing for 1 year
            }
        };

        _subscriptionRepositoryMock.Setup(repo => repo.GetSubscriptionByIdAsync(subscriptionId))
            .ReturnsAsync(subscription);

        _serviceRepositoryMock.Setup(repo => repo.GetServiceByIdAsync(subscription.ServiceId))
            .ReturnsAsync(service);

        // Ensure the payment service mock returns true to simulate a successful payment
        _paymentServiceMock.Setup(service => service.ProcessPaymentAsync(subscription.UserId, 100.0m))
            .ReturnsAsync(true);

        // Act
        var result = await _subscriptionService.ChangeSubscriptionAsync(subscriptionId, newPeriod);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(newPeriod.Period, result.Period.Period);
        Assert.Equal(SubscriptionStatus.Active, result.Status);
        _subscriptionRepositoryMock.Verify(repo => repo.UpdateSubscriptionAsync(It.IsAny<Subscription>()), Times.Once);
    }


    [Fact]
    public async Task ChangeSubscriptionAsync_ShouldThrowException_WhenPaymentFails()
    {
        // Arrange
        var subscriptionId = 1;
        var newPeriod = new SubscriptionPeriod { Period = 365 }; // Assuming 365 days period for 1 year
        var subscription = new Subscription
        {
            Id = subscriptionId,
            ServiceId = 1,
            UserId = 1,
            Period = new SubscriptionPeriod { Period = 30 }, // Original period is 30 days (1 month)
            EndDate = DateTime.UtcNow.AddMonths(1),
            Status = SubscriptionStatus.Active
        };
        var service = new Service
        {
            Id = subscription.ServiceId,
            Name = "Test Service",
            Pricing = new Dictionary<SubscriptionPeriod, decimal>
            {
                { new SubscriptionPeriod { Period = 365 }, 100.0m } // New pricing for 1 year
            }
        };

        _subscriptionRepositoryMock.Setup(repo => repo.GetSubscriptionByIdAsync(subscriptionId))
            .ReturnsAsync(subscription);

        _serviceRepositoryMock.Setup(repo => repo.GetServiceByIdAsync(subscription.ServiceId))
            .ReturnsAsync(service);

        _paymentServiceMock.Setup(service => service.ProcessPaymentAsync(subscription.UserId, 100.0m))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _subscriptionService.ChangeSubscriptionAsync(subscriptionId, newPeriod));

        _subscriptionRepositoryMock.Verify(repo => repo.UpdateSubscriptionAsync(It.IsAny<Subscription>()), Times.Never);
    }

[Fact]
public async Task CancelSubscriptionAsync_ShouldSetSubscriptionStatusToCanceled()
{
    // Arrange
    var subscriptionId = 1;
    var subscription = new Subscription
    {
        Id = subscriptionId,
        ServiceId = 1,
        UserId = 1,
        Period = new SubscriptionPeriod { Period = 30 }, // Original period is 30 days (1 month)
        EndDate = DateTime.UtcNow.AddMonths(1),
        Status = SubscriptionStatus.Active
    };

    _subscriptionRepositoryMock.Setup(repo => repo.GetSubscriptionByIdAsync(subscriptionId))
        .ReturnsAsync(subscription);

    // Act
    await _subscriptionService.CancelSubscriptionAsync(subscriptionId);

    // Assert
    Assert.Equal(SubscriptionStatus.Canceled, subscription.Status);
    _subscriptionRepositoryMock.Verify(repo => repo.UpdateSubscriptionAsync(It.IsAny<Subscription>()), Times.Once);
}

}
