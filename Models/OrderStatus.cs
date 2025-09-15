namespace UShop.Models
{
    public enum OrderStatus
    {
        Pending = 1,
        Ordered = 2,
        Shipped = 3,
        OutForDelivery = 4,
        Delivered = 5,
        Cancelled = 6,
        Returned = 7,
    }
}
