using System;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace GreenLeafMobileService.Helpers {
    public class EmailService : IIdentityMessageService {
        private const int RetryCount = 3;

        public async Task SendAsync ( IdentityMessage message ) {
            var currentTry = 1;
            var delay = 1000;

            while ( true ) {
                try {
                    using ( var smtpClient = new SmtpClient() ) {
                        var mailMessage = new MailMessage {
                            From = new MailAddress( "info@team.neadevis.com" ),
                            To = {new MailAddress( message.Destination )},
                            Subject = message.Subject,
                            Body = message.Body,
                            IsBodyHtml = true
                        };
                        await smtpClient.SendMailAsync( mailMessage ).ConfigureAwait( false );
                        mailMessage.Dispose();
                    }
                    break;
                } catch ( Exception ) {
                    if ( currentTry++ > RetryCount ) {
                        throw;
                    }
                    await Task.Delay( delay );
                    delay *= 2;
                }
            }
        }
    }
}