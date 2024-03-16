using MimeKit;
namespace Accelerate.Foundations.Communication.Models
{
    public class EmailMessage
    {
        public List<MailboxAddress> To { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public IEnumerable<MimePart> Attachments { get; set; }
        public EmailMessage(IEnumerable<string> to, string subject, string content)
        {
            To = new List<MailboxAddress>();
            To.AddRange(to.Select(x => new MailboxAddress("TODO:TO", x)));
            Subject = subject;
            Content = content;
        }
    }
}
