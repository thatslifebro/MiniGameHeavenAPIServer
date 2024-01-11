namespace APIServer.Model.DTO;

public class FriendAddRequest
{
    public int Uid { get; set; }
    public int FriendUid { get; set; }
}


public class FriendAddResponse : ErrorCodeDTO
{
}


