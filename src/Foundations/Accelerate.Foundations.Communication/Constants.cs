using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Communication
{
    public struct Constants
    {
        public struct SendGrid
        {
            public const string SecretPassword = "SendGrid:ApiKey";
        }
        public struct Twilio
        {
            public const string AccountSID = "Twilio:AccountSID";
            public const string AuthToken = "Twilio:AuthToken";
        }
        public struct Settings
        {
            public const string EmailConfiguration = "EmailConfiguration";
            public const string SmsConfiguration = "TwilioConfiguration";
        }

        public struct Templates
        {
            public struct Emails
            {

                public static string CenteredTemplate = $"<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Strict//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd\">" +
                                                        "<html xmlns=\"http://www.w3.org/1999/xhtml\">" +
                                                          "<head>" +
                                                            "<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" />" +
                                                            "<meta name=\"viewport\" content=\"width=device-width\" />" +
                                                            "<!-- NOTE: external links are for testing only -->" +
                                                            "<link href=\"//cdn.muicss.com/mui-0.10.3/email/mui-email-styletag.css\" rel=\"stylesheet\" />" +
                                                            "<link href=\"//cdn.muicss.com/mui-0.10.3/email/mui-email-inline.css\" rel=\"stylesheet\" />" +
                                                          "</head>" +
                                                          "<body>" +
                                                            "<table class=\"mui-body\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\">" +
                                                              "<tr>" +
                                                                "<td>" +
                                                                  "$$emailBody$$" +
                                                                "</td>" +
                                                              "</tr>" +
                                                            "</table>" +
                                                          "</body>" +
                                                        "</html>";
            }
        }
    }
}