namespace Reports.Models
{
    public class CreationResponse : DefaultResponse
    {
        public bool Done { get; set; }

        public int? EntityId { get; set; }
    }
}
