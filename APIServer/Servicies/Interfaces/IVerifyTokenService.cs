using System.Threading.Tasks;
using System;

namespace APIServer.Servicies.Interfaces
{
    public interface IVerifyTokenService
    {
        public Task<bool> VerifyTokenToHive(Int64 playerId, string token);
    }
}
