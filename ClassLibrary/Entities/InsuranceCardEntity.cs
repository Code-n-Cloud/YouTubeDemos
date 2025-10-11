namespace ClassLibrary.Entities
{
    public class InsuranceCardEntity
    {
        public IdNumber IdNumber { get; set; }
        public string Insurer { get; set; }
        public MedicareMedicaidInfo MedicareMedicaidInfo { get; set; }
        public Member Member { get; set; }
        public DateTimeOffset EffectiveDate { get; set; }
        public PrescriptionInfo PrescriptionInfo { get; set; }
        public Subscriber Subscriber { get; set; }
        public Plan Plan { get; set; }
    }
}
