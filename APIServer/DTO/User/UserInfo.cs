using System;

namespace APIServer.DTO.User
{
    public class UserInfoRequest
    {
        public int Uid { get; set; }
    }

    public class UserInfoResponse : ErrorCodeDTO
    {
        public OtherUserInfo UserInfo { get; set; }
    }

    public class OtherUserInfo
    {
        public int uid { get; set; }
        public string nickname { get; set; }
        public int bestscore_ever { get; set; }
        public int bestscore_cur_season { get; set; }
        public int bestscore_prev_season { get; set; }
        public int main_char_key { get; set; }
        public int main_char_skin_key { get; set; }
        public string main_char_costume_json { get; set; }
        public Int64 rank;
    }
}
