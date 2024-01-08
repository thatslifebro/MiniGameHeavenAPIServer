using System;
using System.Threading.Tasks;

namespace APIServer.Repository
{
    public interface IHiveDb : IDisposable
    {
        public Task<ErrorCode> CreateAccountAsync(string email, string pw);

        public Task<Tuple<ErrorCode, Int64>> VerifyUser(string email, string pw);

        public ErrorCode VerifyToken(Int64 player_id, string token);
    }
}
