namespace AuthApi.Models.DTO
{
    public class TokenResponse
    {
        public string? TokenString {get; set;}
        public DateTime validTo {get; set;}
    }
    
}