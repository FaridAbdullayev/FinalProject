namespace HotelUI.Models
{
    public class LoginResponse
    {
        public TokenDetails Token { get; set; }

        public class TokenDetails
        {
            public string Token { get; set; }
            public bool PasswordResetRequired { get; set; }
        }
    }
}
