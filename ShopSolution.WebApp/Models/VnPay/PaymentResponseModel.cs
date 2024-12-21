namespace ShopSolution.WebApp.Models.VnPay
{
    public class PaymentResponseModel
    {
        public string OrderDescription { get; set; }
        public string TransactionId { get; set; }
        public decimal TotalAmount { get; set; }
        public string OrderId { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentId { get; set; }
        public bool Success { get; set; }
        public string Token { get; set; }
        public string VnPayResponseCode { get; set; }

        public string Address { get; set; }
        public decimal ShippingCost { get; set; }

        public string Email {  get; set; }
        public string Phone { get; set; }

    }

}
