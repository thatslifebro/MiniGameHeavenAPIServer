namespace APIServer.Model.DTO;

public class LogoutRequest
{
    public int uid { get; set; }
    public string token { get; set; }
}

public class LogoutResponse
{
    public ErrorCode Result { get; set; } = ErrorCode.None;
}
