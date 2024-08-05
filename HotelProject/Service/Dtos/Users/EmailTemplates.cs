using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Dtos.Users
{
    public class EmailTemplates
    {
        public static string GetDiscountInfoEmail(string email, string flowerName, double oldPrice, double newPrice, string url)
        {
            return $@"
            <html>
            <body>
                <h1>Check out this flower!</h1>
                <p>Dear {email},</p>
                <p>We have a special offer on {flowerName}.</p>
                <p>Old Price: {oldPrice:C}</p>
                <p>New Price: {newPrice:C}</p>
                <p>For more details, click <a href='{url}'>here</a>.</p>
            </body>
            </html>";
        }
    }
}
