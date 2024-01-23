using APIServer.DTO.Game;
using APIServer.Models;
using APIServer.Models.GameDB;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APIServer.Servicies.Interfaces;

public interface IGameService
{
    public Task<(ErrorCode, IEnumerable<GdbGameInfo>)> GetGameList(int uid);
    public Task<ErrorCode> UnlockGame(int uid, int gameId);
    public Task<(ErrorCode, GdbGameInfo)> GetGameInfo(int uid, int gameId);
    public Task<ErrorCode> SaveGame(int uid, int gameId, int score, List<UsedFoodData> foods);
    public Task<(ErrorCode, int)> InitNewUserGameData(Int64 playerId, string nickname);
    public Task<ErrorCode> SetGamePlayChar(int uid, int gameKey, int charKey);
}
 