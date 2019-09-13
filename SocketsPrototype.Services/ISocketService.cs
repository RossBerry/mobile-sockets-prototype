using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using SocketsPrototype.Models;

namespace SocketsPrototype.Services
{
    public interface ISocketService
    {
        event EventHandler<Exception> ErrorEvent;
        event EventHandler<string> InfoEvent;
        event EventHandler<bool> InChannelStarted;
        event EventHandler<bool> OutChannelStarted;
        event EventHandler<DeviceModel> DeviceDetected;

        bool IsListening { get; }
        bool IsSending { get; }

        Task CreateInChannel();

        Task CreateOutChannel();

        void ReadFromInChannel(string text);

        void SendToOutChannel(string text);

        //event EventHandler<DeviceEventArgs> DeviceConnected;

        //event EventHandler<string> InfoEvent;

        //Task ScanForDevices();

        //Task StopScanningForDevices();

        //Task ConnectToDevice(Guid id);
    }
}
