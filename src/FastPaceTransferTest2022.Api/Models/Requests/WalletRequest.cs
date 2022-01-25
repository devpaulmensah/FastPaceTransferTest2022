namespace FastPaceTransferTest2022.Api.Models.Requests
{
    public class WalletRequest
    {
        public string UserId { get; set; }
        public decimal Balance { get; set; }
        public string Currency { get; set; } = "GHS";
    }
}