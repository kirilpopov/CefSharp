namespace CefSharp.Internals
{
    using System;
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
}