using System; 

namespace AuthApi.Models
{
public class GlobalModels
{
public class returnModel
{
 public Object result { get; set; }
 public bool status { get; set; }
 public string error { get; set; }
}
    
public class UserTrafficEntry
  {
  public int Id { get; set; }
  public string? ClientIpAddress { get; set; }
  public DateTime RequestTimeStamp { get; set; }
  public int? ResponseStatusCode { get; set; } 
  public string? RequestBody { get; set; }
  public string? RequestUrl { get; set; }
  public string? ExceptionType { get; set; }
  public string? ExceptionMessage { get; set; }
  public string? ExceptionDetails { get; set; }
}
        
public class RequestDetails
{
 public string? Method { get; set; }
  public string? Body { get; set; }
  public string? RequestUrl { get; set; }
}
}

public class RegisterModel
{
  public string? Username { get; set; }
  public string? Email { get; set; }
  public string? Password { get; set; }
}

public class LoginModel
{
public string? Email { get; set; }
public string? Password { get; set; }
}
}