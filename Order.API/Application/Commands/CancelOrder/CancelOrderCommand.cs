namespace Order.API.Application.Commands.CancelOrder
{
    public class CancelOrderCommand
    {
        public string OrderId { get; set; }
        public string Reason { get; set; }
    }
}
