using Newtonsoft.Json;

namespace Currency.Business.Models
{
    public abstract class Price
    {
        [JsonProperty("last_price_usd")]
        public string LastPrice { get; set; }
        public override string ToString() => LastPrice.Substring(0, LastPrice.LastIndexOf('.') + 3);
    }
}
