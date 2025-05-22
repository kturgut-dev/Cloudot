using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Cloudot.Shared.EntityFramework.Interceptor;

public class SqlLoggingInterceptor : DbCommandInterceptor
{
    public override InterceptionResult<DbDataReader> ReaderExecuting(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<DbDataReader> result)
    {
        Console.WriteLine($"SQL: {command.CommandText}");
        return base.ReaderExecuting(command, eventData, result);
    }
}
