using System.ComponentModel.DataAnnotations;

namespace APIServer.DTO.Friend;

public class FriendAddRequest
{
    [Required]
    public int FriendUid { get; set; }
}


public class FriendAddResponse : ErrorCodeDTO
{
}


