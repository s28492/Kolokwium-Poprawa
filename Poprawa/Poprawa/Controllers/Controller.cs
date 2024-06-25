using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Poprawa.DTO;
using Poprawa.Services;

namespace Poprawa.Controllers;


[ApiController]
[Route("api/clients")]
public class Controller: ControllerBase
{
    private readonly IClientService _clientService;

    public Controller(IClientService clientService)
    {
        _clientService = clientService;
    }
    
    
    [HttpGet("/{id:int}")]
    public async Task<IActionResult> GetClient(int id)
    {
        try
        {
            var client = await _clientService.GetClient(id);
            if (client == null)
            {
                return NotFound();
            }

            return Ok(client);
        }
        catch (Exception e)
        {
            return StatusCode(500);
        }
        
    }

    
    [HttpPost]
    public async Task<IActionResult> AddClientAndRental([FromBody] ClientRentalDTO.ClientRentalDto clientRentalDto)
    {
        if (clientRentalDto == null)
        {
            return BadRequest("You haven't given any data");
        }

        if (clientRentalDto.Rental.DateFrom > clientRentalDto.Rental.DateTo)
        {
            return BadRequest("DateFrom cannot be earlier than dateTo");

        }
        var client = clientRentalDto.Client;
        var rental = clientRentalDto.Rental;
        try
        {
            await _clientService.AddClientWithRental(client, rental);
            return Ok("Client added succesfuly");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error occured while adding new client and rental...");
        }
    }
    
    
}