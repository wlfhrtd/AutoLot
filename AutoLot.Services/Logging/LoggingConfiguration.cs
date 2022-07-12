using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;


namespace AutoLot.Services.Logging
{
    public static class LoggingConfiguration
    {
        // for text file
        private static readonly string _outputTemplate =
            @"[{Timestamp:yy-MM-dd HH:mm:ss} {Level}]{ApplicationName}:{SourceContext}{NewLine}
            Message:{Message}{NewLine}in method {MemberName} at {FilePath}:{LineNumber}{NewLine}
            {Exception}{NewLine}";
        // for db
        private static readonly ColumnOptions _columnOptions = new()
        {
            AdditionalColumns = new List<SqlColumn>
            {
                new() { DataType = SqlDbType.VarChar, ColumnName = "ApplicationName" },
                new() { DataType = SqlDbType.VarChar, ColumnName = "MachineName" },
                new() { DataType = SqlDbType.VarChar, ColumnName = "MemberName" },
                new() { DataType = SqlDbType.VarChar, ColumnName = "FilePath" },
                new() { DataType = SqlDbType.Int, ColumnName = "LineNumber" },
                new() { DataType = SqlDbType.VarChar, ColumnName = "SourceContext" },
                new() { DataType = SqlDbType.VarChar, ColumnName = "RequestPath" },
                new() { DataType = SqlDbType.VarChar, ColumnName = "ActionName" },
            }
        };


        public static IHostBuilder ConfigureSerilog(this IHostBuilder builder)
        {
            return builder
                .ConfigureLogging((context, logging) => { logging.ClearProviders(); })
                .UseSerilog((hostingContext, loggerConfiguration) =>
                {
                    var config = hostingContext.Configuration;
                    string connectionString = config.GetConnectionString("AutoLot").ToString();
                    string tableName = config["Logging:MSSqlServer:tableName"].ToString();
                    string schema = config["Logging:MSSqlServer:schema"].ToString();
                    string restrictedToMinimumLevel = config["Logging:MSSqlServer:restrictedToMinimumLevel"].ToString();

                    if (!Enum.TryParse<LogEventLevel>(restrictedToMinimumLevel, out var logLevel))
                    {
                        logLevel = LogEventLevel.Debug;
                    }
                    LogEventLevel level = (LogEventLevel)Enum.Parse(typeof(LogEventLevel), restrictedToMinimumLevel);

                    MSSqlServerSinkOptions sqlOptions = new()
                    {
                        AutoCreateSqlTable = false,
                        SchemaName = schema,
                        TableName = tableName,
                    };

                    if (hostingContext.HostingEnvironment.IsDevelopment())
                    {
                        sqlOptions.BatchPeriod = new TimeSpan(0, 0, 0, 1);
                        sqlOptions.BatchPostingLimit = 1;
                    }

                    loggerConfiguration
                    .Enrich.FromLogContext()
                    .Enrich.WithMachineName()
                    .WriteTo.File(
                        path: "ErrorLog.txt",
                        rollingInterval: RollingInterval.Day,
                        restrictedToMinimumLevel: logLevel,
                        outputTemplate: _outputTemplate)
                    .WriteTo.Console(restrictedToMinimumLevel: logLevel)
                    .WriteTo.MSSqlServer(
                        connectionString: connectionString,
                        sqlOptions,
                        restrictedToMinimumLevel: level,
                        columnOptions: _columnOptions);
                });
        }
    }
}
