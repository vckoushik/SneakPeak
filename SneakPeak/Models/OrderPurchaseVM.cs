namespace SneakPeak.Models
{
    public class OrderPurchaseVM:Cart
    {
        public Address Address { get; set; }
        public string Nonce { get; set; }
    }
}
