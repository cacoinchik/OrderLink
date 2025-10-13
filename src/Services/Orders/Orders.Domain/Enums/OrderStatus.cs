namespace Orders.Domain.Enums
{
    public enum OrderStatus
    {
        New = 0,
        Reserved = 1,
        PaymentPanding = 2,
        Paid = 3,
        Completed = 4,
        Cancelled = 5,
        Failde = 6,
        Expired = 7
    }
}
