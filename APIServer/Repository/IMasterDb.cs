using APIServer.Model.DAO;
using System;
using System.Threading.Tasks;

namespace APIServer.Repository;

public interface IMasterDb : IDisposable
{
    public VersionDAO? _version { get; }
    public Task<bool> Load();
}
