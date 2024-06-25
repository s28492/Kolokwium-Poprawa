using System.Data.SqlClient;
using Poprawa.DTO;
using Poprawa.Models;

namespace Poprawa.Repositories;
    
public class ClientRepository : IClientRepository
{
    private readonly IConfiguration _configuration;

    public ClientRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<Client> GetClient(int clientId)
    {
        Client client = null;
        var rentals = new List<Rental>();

        var query = @"
            SELECT c.ID, c.FirstName, c.LastName, c.Address, 
                   r.DateFrom, r.DateTo, r.TotalPrice, 
                   car.VIN, col.Name AS Color, m.Name AS Model
            FROM clients c
            INNER JOIN car_rentals r ON c.ID = r.ClientID
            INNER JOIN cars car ON r.CarID = car.ID
            INNER JOIN colors col ON car.ColorID = col.ID
            INNER JOIN models m ON car.ModelID = m.ID
            WHERE c.ID = @ClientId";

        using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        {
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@ClientId", clientId);
                await connection.OpenAsync();

                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        if (client == null)
                        {
                            client = new Client
                            {
                                IdClient = reader.GetInt32(reader.GetOrdinal("ID")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                Address = reader.GetString(reader.GetOrdinal("Address")),
                                Rentals = new List<Rental>()
                            };
                        }

                        client.Rentals.Add(new Rental
                        {
                            Vin = reader.GetString(reader.GetOrdinal("VIN")),
                            Color = reader.GetString(reader.GetOrdinal("Color")),
                            Model = reader.GetString(reader.GetOrdinal("Model")),
                            DateFrom = reader.GetDateTime(reader.GetOrdinal("DateFrom")),
                            DateTo = reader.GetDateTime(reader.GetOrdinal("DateTo")),
                            TotalPrice = reader.GetInt32(reader.GetOrdinal("TotalPrice"))
                        });
                    }
                }
            }
        }

        return client;
    }

    // public async Task<int> GetPrice(int id)
    // {
    //     Car car = null;
    //
    //     var query = @"SELECT PricePerDay FROM car 
    //                      WHERE ID = @CarId";
    //     using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
    //     {
    //         using (SqlCommand command = new SqlCommand(query, connection))
    //         {
    //             Console.WriteLine(1);
    //             command.Parameters.AddWithValue("@CarId", id);
    //             await connection.OpenAsync();
    //             Console.WriteLine(2);
    //             using (SqlDataReader reader = await command.ExecuteReaderAsync())
    //             {
    //                 Console.WriteLine("Excecution query success");
    //                 if (reader.Read())
    //                 {
    //                     Console.WriteLine(3);
    //                     if (car == null)
    //                     {
    //                         Console.WriteLine("Proba");
    //                         car = new Car
    //                         {
    //                             CarId = id,
    //                             Price = reader.GetInt32(reader.GetOrdinal("Price"))
    //                         };
    //                     }
    //                 }
    //             }
    //         }
    //
    //     }
    //     return car.Price;
    // }

    public async Task<bool> AddClientWithRental(ClientRentalDTO.ClientDto client, ClientRentalDTO.RentalDto rental)
    {
        int clientId = 0;
        var queryClient = @"
            INSERT INTO clients (FirstName, LastName, Address)
            OUTPUT INSERTED.ID
            VALUES (@FirstName, @LastName, @Address)";

        var queryRental = @"
            INSERT INTO car_rentals (ClientID, CarID, DateFrom, DateTo, TotalPrice)
            VALUES (@ClientID, @CarID, @DateFrom, @DateTo, @TotalPrice)";

        using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        {
            await connection.OpenAsync();
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    using (var command = new SqlCommand(queryClient, connection, transaction))
                    {
                        command.Parameters.AddWithValue("@FirstName", client.FirstName);
                        command.Parameters.AddWithValue("@LastName", client.LastName);
                        command.Parameters.AddWithValue("@Address", client.Address);

                        clientId = (int)await command.ExecuteScalarAsync();
                    }

                    // int price = await GetPrice(rental.CarId);


                    if (clientId > 0)
                    {
                        using (var command = new SqlCommand(queryRental, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@ClientID", clientId);
                            command.Parameters.AddWithValue("@CarID", rental.CarId);
                            command.Parameters.AddWithValue("@DateFrom", rental.DateFrom);
                            command.Parameters.AddWithValue("@DateTo", rental.DateTo);
                            command.Parameters.AddWithValue("@TotalPrice", rental.TotalPrice);

                            await command.ExecuteNonQueryAsync();
                        }
                        Console.WriteLine(3);
                        transaction.Commit();
                    }
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        return true;
    }
}
