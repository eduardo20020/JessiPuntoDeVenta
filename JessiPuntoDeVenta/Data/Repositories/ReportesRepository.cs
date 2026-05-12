using Dapper;
using MySqlConnector;

namespace JessiPuntoDeVenta.Data.Repositories;

public sealed class ReportesRepository : IReportesRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public ReportesRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<ResumenDiaDto>> ResumenPorDiaAsync(
        DateTime desdeInicioDia,
        DateTime hastaExclusivo,
        CancellationToken cancellationToken = default)
    {
        await using var conn = (MySqlConnection)_connectionFactory.CreateConnection();
        await conn.OpenAsync(cancellationToken);
        const string sql = """
            SELECT DATE(v.fecha_hora) AS Dia,
                   SUM(lv.importe_linea) AS VentasTotales,
                   SUM(IFNULL(lv.costo_unitario, 0) * lv.cantidad) AS CostoTotal,
                   SUM(lv.importe_linea - IFNULL(lv.costo_unitario, 0) * lv.cantidad) AS MargenBruto
            FROM ventas v
            INNER JOIN lineas_venta lv ON lv.id_venta = v.id
            WHERE v.fecha_hora >= @desdeInicioDia AND v.fecha_hora < @hastaExclusivo
            GROUP BY DATE(v.fecha_hora)
            ORDER BY Dia
            """;
        var rows = await conn.QueryAsync<ResumenDiaDto>(
            new CommandDefinition(sql, new { desdeInicioDia, hastaExclusivo }, cancellationToken: cancellationToken));
        return rows.AsList();
    }

    public async Task<ResumenDiaDto?> TotalesEnRangoAsync(
        DateTime desdeInicioDia,
        DateTime hastaExclusivo,
        CancellationToken cancellationToken = default)
    {
        await using var conn = (MySqlConnection)_connectionFactory.CreateConnection();
        await conn.OpenAsync(cancellationToken);
        const string sql = """
            SELECT COALESCE(SUM(lv.importe_linea), 0) AS VentasTotales,
                   COALESCE(SUM(IFNULL(lv.costo_unitario, 0) * lv.cantidad), 0) AS CostoTotal,
                   COALESCE(SUM(lv.importe_linea - IFNULL(lv.costo_unitario, 0) * lv.cantidad), 0) AS MargenBruto
            FROM ventas v
            INNER JOIN lineas_venta lv ON lv.id_venta = v.id
            WHERE v.fecha_hora >= @desdeInicioDia AND v.fecha_hora < @hastaExclusivo
            """;
        return await conn.QuerySingleOrDefaultAsync<ResumenDiaDto>(
            new CommandDefinition(sql, new { desdeInicioDia, hastaExclusivo }, cancellationToken: cancellationToken));
    }
}
