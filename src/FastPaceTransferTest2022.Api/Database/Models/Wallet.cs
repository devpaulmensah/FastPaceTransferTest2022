using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FastPaceTransferTest2022.Api.Database.Models
{
    public class Wallet
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public string UserId { get; set; }
        public decimal Balance { get; set; }
        public string Currency { get; set; } = "GHS";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}