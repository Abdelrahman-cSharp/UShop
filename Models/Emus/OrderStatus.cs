namespace UShop.Models
{
    public enum OrderStatus
    {
        Pending = 0,
        Ordered = 1,
        Shipped = 2,
        OutForDelivery = 3,
        Delivered = 4,
        Cancelled = 5,
        Returned = 6,
    }
}
