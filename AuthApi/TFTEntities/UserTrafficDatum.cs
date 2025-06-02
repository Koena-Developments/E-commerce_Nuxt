using System;
using System.Collections.Generic;

namespace AuthApi.TFTEntities;

public partial class UserTrafficDatum
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
