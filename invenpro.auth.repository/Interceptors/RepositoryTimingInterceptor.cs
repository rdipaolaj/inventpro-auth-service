using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using System.Data.Common;

namespace invenpro.auth.repository.Interceptors;

/// <summary>
/// Intercepta cada comando que EF Core envía a la base de datos
/// y registra en el log cuánto tiempo tardó en ejecutarse.
/// </summary>
public class RepositoryTimingInterceptor : DbCommandInterceptor
{
    private readonly ILogger<RepositoryTimingInterceptor> _logger;

    public RepositoryTimingInterceptor(ILogger<RepositoryTimingInterceptor> logger)
    {
        _logger = logger;
    }

    //
    // === SELECTs que devuelven un DbDataReader ===
    //

    public override DbDataReader ReaderExecuted(DbCommand command, CommandExecutedEventData eventData, DbDataReader result)
    {
        LogCommandTiming(command.CommandText, eventData.Duration.TotalMilliseconds);
        return base.ReaderExecuted(command, eventData, result);
    }

    public override ValueTask<DbDataReader> ReaderExecutedAsync(DbCommand command, CommandExecutedEventData eventData, DbDataReader result, CancellationToken cancellationToken = default)
    {
        LogCommandTiming(command.CommandText, eventData.Duration.TotalMilliseconds);
        return base.ReaderExecutedAsync(command, eventData, result, cancellationToken);
    }

    //
    // === INSERT/UPDATE/DELETE (NonQuery) ===
    //

    public override int NonQueryExecuted(DbCommand command, CommandExecutedEventData eventData, int result)
    {
        LogCommandTiming(command.CommandText, eventData.Duration.TotalMilliseconds);
        return base.NonQueryExecuted(command, eventData, result);
    }

    public override ValueTask<int> NonQueryExecutedAsync(DbCommand command, CommandExecutedEventData eventData, int result, CancellationToken cancellationToken = default)
    {
        LogCommandTiming(command.CommandText, eventData.Duration.TotalMilliseconds);
        return base.NonQueryExecutedAsync(command, eventData, result, cancellationToken);
    }

    //
    // === Scalar (ej. COUNT(*)) ===
    //

    public override object? ScalarExecuted(DbCommand command, CommandExecutedEventData eventData, object? result)
    {
        LogCommandTiming(command.CommandText, eventData.Duration.TotalMilliseconds);
        return base.ScalarExecuted(command, eventData, result);
    }

    public override ValueTask<object?> ScalarExecutedAsync(DbCommand command, CommandExecutedEventData eventData, object? result, CancellationToken cancellationToken = default)
    {
        LogCommandTiming(command.CommandText, eventData.Duration.TotalMilliseconds);
        return base.ScalarExecutedAsync(command, eventData, result, cancellationToken);
    }

    /// <summary>
    /// Escribe en el log la información del comando y su duración.
    /// </summary>
    private void LogCommandTiming(string commandText, double elapsedMilliseconds)
    {
        _logger.LogInformation("EF Core Command Timing: {ElapsedMilliseconds} ms | {CommandText}", elapsedMilliseconds, TruncateForLog(commandText));
    }

    /// <summary>
    /// Recorta el SQL para no saturar el log con un texto muy largo.
    /// </summary>
    private static string TruncateForLog(string sql, int maxLength = 200)
    {
        if (string.IsNullOrEmpty(sql) || sql.Length <= maxLength)
            return sql.Replace(Environment.NewLine, " ");
        return sql.Substring(0, maxLength).Replace(Environment.NewLine, " ") + "...";
    }
}