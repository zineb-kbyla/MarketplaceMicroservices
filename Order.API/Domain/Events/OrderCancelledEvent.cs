namespace Order.API.Domain.Events
{
    public class OrderCancelledEvent
    {
        public string OrderId { get; set; }
        public string Reason { get; set; }
        public DateTime CancelledAt { get; set; }
    }
}
