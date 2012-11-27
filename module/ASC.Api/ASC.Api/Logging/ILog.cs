using System;

namespace ASC.Api.Logging
{
    public interface ILog
    {
        void Fatal(Exception error, string format, params object[] args);
        void Error(Exception error, string format, params object[] args);
        void Warn(string format, params object[] args);
        void Info(string format, params object[] args);
        void Debug(string format, params object[] args);

        bool IsDebugEnabled { get; }
    }
}