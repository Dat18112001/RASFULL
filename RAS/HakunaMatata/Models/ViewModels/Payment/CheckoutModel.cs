namespace HakunaMatata.Models.ViewModels
{
    public class CheckoutModel
    {
        public string PaymentMethod { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Note { get; set; }
        public double TotalAmount { get; set; }
    }
}