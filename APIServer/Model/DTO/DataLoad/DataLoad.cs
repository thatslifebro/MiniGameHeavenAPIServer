using APIServer.Model.DAO.GameDB;
using System.Collections.Generic;

namespace APIServer.Model.DTO.DataLoad
{
    public class DataLoadResponse : ErrorCodeDTO
    {
        public GdbUserInfo UserInfo { get; set; }
        public IEnumerable<FriendUserInfo> FriendList { get; set; }
        public IEnumerable<GdbGameInfo> GameList { get; set; }
        public IEnumerable<UserCharInfo> CharList { get; set; }
        public IEnumerable<GdbUserSkinInfo> SkinList { get; set; }
        public IEnumerable<GdbUserCostumeInfo> CostumeList { get; set; }
        public IEnumerable<GdbUserFoodInfo> FoodList { get; set; }
        public IEnumerable<UserMailInfo> MailList { get; set; }
        public GdbAttendanceInfo AttendanceInfo { get; set; }
    }


    public class UserCharInfo
    {
        public GdbUserCharInfo CharInfo { get; set; }
        public IEnumerable<GdbUserCharRandomSkillInfo> RandomSkills { get; set; }
    }

    public class UserMailInfo
    {
        public GdbMailboxInfo MailInfo { get; set; }
        public IEnumerable<GdbMailboxRewardInfo> MailItems { get; set; }
    }
}
