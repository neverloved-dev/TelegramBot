using TelegramBot.Models;

namespace TelegramBot.Interfaces;

public interface IServiceRepository
{
    Task<Service> GetServiceByIdAsync(int id);
    Task<IEnumerable<Service>> GetAllServicesAsync();
}