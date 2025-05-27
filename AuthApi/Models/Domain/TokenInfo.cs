namespace AuthApi.Models.Domain
{
    public class TokenInfo
    {
        public int id  { get; set; }
        public string USername {get; set;}
        public string RefreshToken {get; set;}
        public DateTime RefreshTokenExpirary {get; set;}
    }
}