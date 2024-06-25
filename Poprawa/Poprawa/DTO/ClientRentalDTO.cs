namespace Poprawa.DTO;

public class ClientRentalDTO
{
    public class ClientRentalDto
    {
        public ClientDto Client { get; set; }
        public RentalDto Rental { get; set; }
    }

    public class ClientDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
    }

    public class RentalDto
    {
        public int CarId { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public int TotalPrice { get; set; }
    }
}