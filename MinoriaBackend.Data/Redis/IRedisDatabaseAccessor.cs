using StackExchange.Redis;

namespace MinoriaBackend.Data.Redis;

public interface IRedisDatabaseAccessor
{
    IDatabase GetDatabase(int dbNumber = -1);
    IServer GetServer();
}