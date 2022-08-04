namespace TourismSmartTransportation.Business.CommonModel
{
    public class TwilioSettings : ITwilioSettings
    {
        public TwilioSettings()
        {
            _authToken = System.Environment.GetEnvironmentVariable("AuthToken");
        }
        private string _authToken;
        public string AccountSid { get; set; }
        public string AuthToken { get { return _authToken; } }
        public string RequestId { get; set; }
        public string PhoneNumber { get; set; }
    }

    public interface ITwilioSettings
    {
        string AccountSid { get; set; }
        string AuthToken { get; }
        string RequestId { get; set; }
        string PhoneNumber { get; set; }
    }
}
