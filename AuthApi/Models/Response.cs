namespace AuthApi.Models
{
public class ResponseModel
{
 public int Newstatus { get; set; }
 public string? Message { get; set; }
public object? Result { get; set; }
 public string? Ip { get; set; }
 public string? Body { get; set; }
 public string? RequestMethod { get; set; }
    }
}