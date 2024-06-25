using Poprawa.DTO;
using Poprawa.Models;

namespace Poprawa.Services;

public interface IClientService
{
    Task<Client> GetClient(int clientId);

    Task AddClientWithRental(ClientRentalDTO.ClientDto clientDto, ClientRentalDTO.RentalDto rentalDto);
}