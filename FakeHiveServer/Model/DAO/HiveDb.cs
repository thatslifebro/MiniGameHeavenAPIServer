using System;

namespace APIServer.Model.DAO;

//AccountDB의 객체는 객체 이름 앞에 Adb를 붙인다.

public class HdbAccountInsert
{
    public Int32? player_id { get; set; }
    public string email { get; set; }
    public string hashed_pw { get; set; }
    public string salt_value { get; set; }
    public string? recent_login_dt { get; set; }
    public string? create_dt { get; set; }
}

public class HdbAccount
{
    public Int32 player_id { get; set; }
    public string email { get; set; }
    public string hashed_pw { get; set; }
    public string salt_value { get; set; }
    public string? recent_login_dt { get; set; }
    public string create_dt { get; set; }
}