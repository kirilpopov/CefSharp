namespace CefSharp.Internals
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.ServiceModel;

    [ServiceContract(SessionMode = SessionMode.Required)]
    [ServiceKnownType(typeof(Exception))]
    [ServiceKnownType(typeof(LogMessage))]
    [ServiceKnownType(typeof(StartupArgumentsLogMessage))]
    public interface ILogService
    {      
        [OperationContract]
        void Log(LogMessage message);       
    }

    [DataContract]
    public class LogMessage
    {
        public LogMessage(string message)
            :this(message, null)
        {            
        }

        public LogMessage(Exception exception)
            :this(null, exception)
        {                        
        }

        public LogMessage(string message, Exception exception)
        {
            Message = message;
            Exception = exception;
        }

        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public Exception Exception { get; set; }

        [DataMember]
        public int BrowserId { get; set; }

        public override string ToString()
        {
            return string.Format("Message: {0}, Exception: {1}", Message, Exception);
        }
    }

    [DataContract]
    public class StartupArgumentsLogMessage : LogMessage
    {
        [DataMember]
        public IEnumerable<string> Arguments { get; set; }

        public StartupArgumentsLogMessage(IEnumerable<string> arguments) : base("Startup arguments")
        {
            Arguments = arguments;
        }
    }
}