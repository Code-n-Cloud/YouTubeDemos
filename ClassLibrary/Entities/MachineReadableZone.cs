namespace ClassLibrary.Entities
{
    public class MachineReadableZone
    {
        public string Country { get; set; }
        public DateTimeOffset DateOfBirth { get; set; }
        public DateTimeOffset DateOfExpiration { get; set; }
        public string DocumentNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Nationality { get; set; }
        public string Sex { get; set; }
    }
}
