namespace APIServer.Model.DAO;

//GameDB의 객체는 객체 이름 앞에 Gdb를 붙인다.

public class GdbGameInfo
{
    public int game_id { get; set; }
    public string game_name { get; set; }
    public int bestscore { get; set; }
    public string create_dt { get; set; }
}
