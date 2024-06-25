using Poprawa.DTO;
using Poprawa.Models;

namespace Poprawa.Repositories;

public interface IClientRepository
{
    Task<Client> GetClient(int clientId);
    // Task<int> GetPrice(int id);
    Task<bool> AddClientWithRental(ClientRentalDTO.ClientDto client, ClientRentalDTO.RentalDto rental);
}