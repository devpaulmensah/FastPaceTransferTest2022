using System;

namespace FastPaceTransferTest2022.Api.Models.Responses
{
    public class WalletResponse
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public decimal Balance { get; set; }
        public string Currency { get; set; } = "GHS";
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}