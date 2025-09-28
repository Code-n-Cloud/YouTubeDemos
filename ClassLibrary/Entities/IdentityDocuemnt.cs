using Azure.AI.DocumentIntelligence;

namespace ClassLibrary.Entities
{
    public class IdentityDocuemnt
    {
        public AddressValue Address { get; set; }
        public string Country { get; set; }
        public DateTimeOffset DateOfIssue { get; set; }
        public string DocumentNumber { get; set; }
        public DateTimeOffset DateOfBirth { get; set; }
        public DateTimeOffset DateOfExpiration { get; set; }
        public string DocumentDiscriminator { get; set; }
        public string Endorsements { get; set; }
        public string EyeColor { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Height { get; set; }
        public string Region { get; set; }
        public string Restrictions { get; set; }
        public string Sex { get; set; }
        public string VehicleClassifications { get; set; }
        public string Weight { get; set; }
        public string PersonalNumber { get; set; }
        public string PlaceOfBirth { get; set; }
        public string Category { get; set; }
        public string DocumentType { get; set; }
        public string IssuingAuthority { get; set; }
        public string Nationality { get; set; }
        public MachineReadableZone MachineReadableZone { get; set; }
    }
}
