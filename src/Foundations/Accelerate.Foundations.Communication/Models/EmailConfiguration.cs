namespace Accelerate.Foundations.Communication.Models
{
    public class EmailConfiguration
    {
        public string FromDomain { get; set; }
        public string From { get; set; }
        public string SmtpServer { get; set; }
        public int Port { get; set; }
        public bool SSL { get; set; }
        public string Key { get; set; }
        public string Username { get; set; }
        public string Password { get; set; } 
    }
}
