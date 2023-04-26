using SimpleAdsAuth.Data;

namespace SimpleAdsAuth4_19.Web.Models
{
    public class IndexViewModel
    {
        public List<Ad> Ads { get; set; }
        public int? CurrentUserId { get; set; }
    }
}
