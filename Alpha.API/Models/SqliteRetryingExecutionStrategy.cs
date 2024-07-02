using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;

public class SqliteRetryingExecutionStrategy : ExecutionStrategy
{
    public SqliteRetryingExecutionStrategy(ExecutionStrategyDependencies dependencies, int maxRetryCount, TimeSpan maxRetryDelay)
        : base(dependencies, maxRetryCount, maxRetryDelay)
    {
    }

    protected override bool ShouldRetryOn(Exception exception)
    {
        if (exception is Microsoft.Data.Sqlite.SqliteException sqliteException)
        {
            // SQLite error code 5 indicates a database is locked
            return sqliteException.SqliteErrorCode == 5;
        }
        return false;
    }
}
