using APIServer.Controllers.Game;
using APIServer.Model.DAO.GameDB;
using APIServer.Model.DTO.Friend;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APIServer.Servicies.Interfaces;

public interface IGameService
{
    public Task<(ErrorCode, IEnumerable<GdbGameInfo>)> GetGameList(int uid);
    public Task<ErrorCode> UnlockGame(int uid, int gameId);
    public Task<(ErrorCode, GdbGameInfo)> GetGameInfo(int uid, int gameId);
    public Task<ErrorCode> SaveGame(int uid, int gameId, int score);
    public Task<ErrorCode> InitNewUserGameData(int uid);
}
