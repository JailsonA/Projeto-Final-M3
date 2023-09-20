using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Filters
{
    public class emailTemplate
    {
        public string resTpwd(string envEmail)
        {
            string message = @"
                <!DOCTYPE html>
                <html lang=""en"">
                <head>
                    <meta charset=""UTF-8"">
                    <meta http-equiv=""X-UA-Compatible"" content=""IE=edge"">
                    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                    <title>Password Recovery</title>
                </head>
                <body>
                    <p>Dear User,</p>
    
                    <p>We received a request to reset your password. Your new password is:</p>

                    <p><strong>" + envEmail + @"</strong></p>
                    <p><strong>After loggin, change your password for security reasons.</strong></p>
    
                    <p>If you did not request this change or have any concerns about your account's security, please contact our support team immediately.</p>

                    <p>Best regards,<br>Dev. Janilson Andrade & Dev Wagner Baptista</p>
                </body>
                </html>";
            return message;
        }
    }
}
