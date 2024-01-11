using System.ComponentModel.DataAnnotations;

namespace APIServer.Model.DTO.Friend;

public class FriendDeleteRequest : RequestDTO
{
    [Required]
    public int FriendUid { get; set; }
}


public class FriendDeleteResponse : ErrorCodeDTO
{
}


