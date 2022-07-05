namespace TourismSmartTransportation.Business.CommonModel
{
    public class TwilioSettings : ITwilioSettings
    {
        public TwilioSettings() { }
        public string AccountSid { get; set; }
        public string AuthToken { get; set; }
        public string RequestId { get; set; }
    }

    public interface ITwilioSettings
    {
        string AccountSid { get; set; }
        string AuthToken { get; set; }
        string RequestId { get; set; }
    }
}
