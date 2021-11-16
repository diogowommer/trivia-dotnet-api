using System.Data;

namespace CrossCutting.SeedWork.Interfaces
{
    public interface IDatabaseConnectionFactory
    {
        IDbConnection GetConnection();
    }
}
