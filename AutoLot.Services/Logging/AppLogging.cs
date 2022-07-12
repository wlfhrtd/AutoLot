using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog.Context;


namespace AutoLot.Services.Logging
{
    public class AppLogging<T> : IAppLogging<T>
    {
        private readonly ILogger<T> _logger;
        private readonly IConfiguration _config;
        private readonly string _applicationName;


        public AppLogging(ILogger<T> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
            _applicationName = config.GetValue<string>("ApplicationName");
        }


        public void LogAppCritical(
            Exception exception,
            string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var list = PushProperties(memberName, sourceFilePath, sourceLineNumber);
            _logger.LogError(exception, message);
            foreach (var x in list) x.Dispose();
        }

        public void LogAppCritical(
            string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var list = PushProperties(memberName, sourceFilePath, sourceLineNumber);
            _logger.LogError(message);
            foreach (var x in list) x.Dispose();
        }

        public void LogAppDebug(
            string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var list = PushProperties(memberName, sourceFilePath, sourceLineNumber);
            _logger.LogError(message);
            foreach (var x in list) x.Dispose();
        }

        public void LogAppError(
            Exception exception,
            string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var list = PushProperties(memberName, sourceFilePath, sourceLineNumber);
            _logger.LogError(exception, message);
            foreach (var x in list) x.Dispose();
        }

        public void LogAppError(
            string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var list = PushProperties(memberName, sourceFilePath, sourceLineNumber);
            _logger.LogError(message);
            foreach (var x in list) x.Dispose();
        }

        public void LogAppInformation(
            string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var list = PushProperties(memberName, sourceFilePath, sourceLineNumber);
            _logger.LogError(message);
            foreach (var x in list) x.Dispose();
        }

        public void LogAppTrace(
            string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var list = PushProperties(memberName, sourceFilePath, sourceLineNumber);
            _logger.LogError(message);
            foreach (var x in list) x.Dispose();
        }

        public void LogAppWarning(
            string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var list = PushProperties(memberName, sourceFilePath, sourceLineNumber);
            _logger.LogError(message);
            foreach (var x in list) x.Dispose();
        }

        internal List<IDisposable> PushProperties(string memberName, string sourceFilePath, int sourceLineNumber)
        {
            return new()
            {
                LogContext.PushProperty("MemberName", memberName),
                LogContext.PushProperty("FilePath", sourceFilePath),
                LogContext.PushProperty("LineNumber", sourceLineNumber),
                LogContext.PushProperty("ApplicationName", _applicationName),
            };
        } 
    }
}
