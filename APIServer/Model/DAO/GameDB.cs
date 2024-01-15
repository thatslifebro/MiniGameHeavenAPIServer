using System;

namespace APIServer.Model.DAO;

//GameDB의 객체는 객체 이름 앞에 Gdb를 붙인다.

public class GdbGameListInfo
{
    public int game_id { get; set; }
    public string game_name { get; set; }
    public int bestscore { get; set; }
    public DateTime create_dt { get; set; }
}

public class GdbGameInfo
{
    public int game_id { get; set; }
    public string game_name { get; set; }
    public int bestscore { get; set; }
    public DateTime create_dt { get; set; }
    public DateTime new_record_dt { get; set; }
    public DateTime recent_play_dt { get; set; }
    public int bestscore_cur_season { get; set; }
    public int bestscore_prev_season { get; set; }
    // 아이템1 레벨 및 개수, 아이템2 레벨 및 개수, 대표 캐릭터 정보(레벨, 코스튬) 추가예정
}
