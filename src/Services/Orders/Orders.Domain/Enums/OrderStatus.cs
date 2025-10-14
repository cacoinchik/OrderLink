namespace Orders.Domain.Enums
{
    public enum OrderStatus
    {
        New = 0,
        Reserved = 1,
        PaymentPending = 2,
        Paid = 3,
        Completed = 4,
        Cancelled = 5,
        Failed = 6,
        Expired = 7
    }
}
