namespace FastPaceTransferTest2022.Api.Configurations
{
    public class BearerTokenConfig
    {
        public string Audience { get; set; }
        public string Issuer { get; set; }
        public string Key { get; set; }
        public int ExpiryDays { get; set; }
    }
}