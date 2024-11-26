using System;

namespace fasttool_modern.Services.Interfaces
{
    public interface IConnectionManager
    {
        
        void Send(string message);
        void GetInfoDevice();
        bool AskConnection();
        void Connect();
        void Disconnect();
        event Action<string> DataReceived;
    }
}
