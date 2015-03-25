namespace CefSharp.Internals
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [DataContract]
    public class LogMessage
    {
        public LogMessage(string message, LogSeverity severity = LogSeverity.Info)
            : this(message, null, severity)
        {
        }

        public LogMessage(Exception exception, LogSeverity severity = LogSeverity.Error)
            : this(null, exception, severity)
        {
        }

        public LogMessage(string message, Exception exception, LogSeverity severity = LogSeverity.Error)
        {
            Message = message;
            Exception = exception;
            Severity = severity;
        }

        [DataMember]
        public LogSeverity Severity { get; set; }

        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public Exception Exception { get; set; }

        [DataMember]
        public int BrowserId { get; set; }

        public override string ToString()
        {
            if (Exception == null)
            {
                return string.Format("{0} {1}", Severity, Message);
            }
            return string.Format("{0} {1}, Exception: {2}", Severity, Message, Exception);
        }
    }

    [DataContract]
    public class StartupArgumentsLogMessage : LogMessage
    {
        [DataMember]
        public IEnumerable<string> Arguments { get; set; }

        public StartupArgumentsLogMessage(IEnumerable<string> arguments)
            : base("Startup arguments")
        {
            Arguments = arguments;
        }

        public override string ToString()
        {
            var args = string.Empty;
            if (Arguments != null)
            {
                args = String.Join(",", Arguments);
            }

            return base.ToString() + " args:" + args;
        }
    }
}
