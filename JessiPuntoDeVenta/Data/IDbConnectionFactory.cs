using System.Data;

namespace JessiPuntoDeVenta.Data;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}
