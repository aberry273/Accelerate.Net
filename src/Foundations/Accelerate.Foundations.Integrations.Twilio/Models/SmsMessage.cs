using TwilioSDK = Twilio;
namespace Accelerate.Foundations.Integrations.Twilio.Models
{
    public class SmsMessage
    {

        public List<TwilioSDK.Types.PhoneNumber> ToMany { get; set; }
        public TwilioSDK.Types.PhoneNumber To { get; set; }
        public TwilioSDK.Types.PhoneNumber From { get; set; }
        public string Content { get; set; }
        public SmsMessage(IEnumerable<string> to, string from, string content)
        {
            ToMany = to.Select(x => new TwilioSDK.Types.PhoneNumber(x)).ToList();
            From = new TwilioSDK.Types.PhoneNumber(from);
            Content = content;
        }
        public SmsMessage(string to, string from, string content)
        {
            To = new TwilioSDK.Types.PhoneNumber(to);
            From = new TwilioSDK.Types.PhoneNumber(from);
            Content = content;
        }
    }
}
