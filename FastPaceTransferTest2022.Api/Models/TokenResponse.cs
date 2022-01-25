namespace FastPaceTransferTest2022.Api.Models
{
    public class GenerateTokenResponse
    {
        public string BearerToken { get; set; }
        public int? Expiry { get; set; }
    }
}