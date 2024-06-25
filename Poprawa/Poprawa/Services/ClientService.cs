using Poprawa.DTO;
using Poprawa.Models;
using Poprawa.Repositories;

namespace Poprawa.Services;

public class ClientService: IClientService
{
    private readonly IClientRepository _clientRepository;

    public ClientService(IClientRepository clientRepository)
    {
        _clientRepository = clientRepository;
    }

    public async Task<Client> GetClient(int clientId)
    {
        var client = await _clientRepository.GetClient(clientId);
        return client;
    }

    public async Task AddClientWithRental(ClientRentalDTO.ClientDto clientDto, ClientRentalDTO.RentalDto rentalDto)
    {
        await _clientRepository.AddClientWithRental(clientDto, rentalDto);
    }
}