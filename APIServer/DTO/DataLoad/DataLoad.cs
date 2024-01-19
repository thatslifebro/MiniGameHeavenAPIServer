using APIServer.Models.GameDB;
using System.Collections.Generic;

namespace APIServer.DTO.DataLoad
{
    public class DataLoadResponse : ErrorCodeDTO
    {
        public UserDataLoadInfo UserData { get; set; }
    }

    public class UserDataLoadInfo
    {
        public GdbUserInfo UserInfo { get; set; }
        public GdbUserMoneyInfo MoneyInfo { get; set; }
        public IEnumerable<FriendUserInfo> FriendList { get; set; }
        public IEnumerable<GdbGameInfo> GameList { get; set; }
        public List<UserCharInfo> CharList { get; set; }
        public IEnumerable<GdbUserSkinInfo> SkinList { get; set; }
        public IEnumerable<GdbUserCostumeInfo> CostumeList { get; set; }
        public IEnumerable<GdbUserFoodInfo> FoodList { get; set; }
        public List<UserMailInfo> MailList { get; set; }
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
