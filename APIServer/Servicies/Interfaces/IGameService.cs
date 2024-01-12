using APIServer.Model.DAO;
using APIServer.Model.DTO.Friend;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APIServer.Servicies.Interfaces;

public interface IGameService
{
    public Task<(ErrorCode, IEnumerable<GdbGameInfo>)> GetGameList(int uid);
    public Task<ErrorCode> InitGameList(int uid);
    public Task<ErrorCode> UnlockGame(int uid, int gameId);
}
