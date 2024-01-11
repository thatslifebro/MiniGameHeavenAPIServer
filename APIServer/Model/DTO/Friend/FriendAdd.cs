using System.ComponentModel.DataAnnotations;

namespace APIServer.Model.DTO.Friend;

public class FriendAddRequest : RequestDTO
{
    [Required]
    public int FriendUid { get; set; }
}


public class FriendAddResponse : ErrorCodeDTO
{
}


