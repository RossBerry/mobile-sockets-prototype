using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SocketsPrototype.Models;
using Sockets.Plugin;

namespace SocketsPrototype.Services
{
    public class SocketService : ISocketService
    {
        int ISocketService.inPort => 9000;

        public event EventHandler<Exception> ErrorEvent;
        public event EventHandler<string> InfoEvent;
        public event EventHandler<bool> InChannelStarted;
        public event EventHandler<bool> OutChannelStarted;
        public event EventHandler<DeviceModel> DeviceDetected;
        public bool IsListening { get; private set; }
        public bool IsSending { get; private set; }


        private int inPort = 9000;
        private TcpSocketListener _inChannel = null;
        private TcpSocketClient _outChannel = null;
        

        public async Task Listen(int port)
        {

            var myIP = GetLocalIPAddress();
            _inChannel = new TcpSocketListener();

            // when we get connections, read byte-by-byte from the socket's read stream
            _inChannel.ConnectionReceived += async (sender, args) =>
            {
                RaiseInfoEvent("Client Connected!");
                var client = args.SocketClient;

                var bytesRead = -1;
                var buf = new byte[1];

                while (bytesRead != 0)
                {
                    bytesRead = await args.SocketClient.ReadStream.ReadAsync(buf, 0, 1);
                    if (bytesRead > 0)
                        Debug.Write(buf[0]);
                        RaiseInfoEvent(string.Format("Client sent: {0}", buf[0]));
                }
            };

            // bind to the listen port across all interfaces
            await _inChannel.StartListeningAsync(inPort);

            IsListening = true;
            RaiseInfoEvent(string.Format("Listening at {0}", myIP));
            RaiseInChannelStarted(true);
        }

        public async Task Send(string ip, int port)
        {
            var r = new Random();
            
            RaiseInfoEvent(string.Format("Connecting to: {0}:{1}", ip, port));
            _outChannel = new TcpSocketClient();
            RaiseOutChannelStarted(true);

            await _outChannel.ConnectAsync(ip, port);
            RaiseInfoEvent("We're connected!");
            
            for (int i = 0; i < 5; i++)
            {
                
                // write to the 'WriteStream' property of the socket client to send data
                var nextByte = (byte)r.Next(0, 254);
                RaiseInfoEvent(string.Format("Sending: {0}", nextByte));
                _outChannel.WriteStream.WriteByte(nextByte);
                await _outChannel.WriteStream.FlushAsync();

                // wait a little before sending the next bit of data
                await Task.Delay(TimeSpan.FromMilliseconds(500));
            }

            await _outChannel.DisconnectAsync();
            RaiseInfoEvent("Message sent!");
            RaiseOutChannelStarted(false);
            IsSending = false;
        }

        public Task ScanForDevices()
        {
            throw new NotImplementedException();

            // TODO: Scan for UDP broadcasts and add devices to DetectedDevices
        }

        public string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        private void RaiseErrorEvent(Exception e)
        {
            ErrorEvent?.Invoke(this, e);
        }

        private void RaiseInfoEvent(string info)
        {
            InfoEvent?.Invoke(this, info);
        }

        private void RaiseInChannelStarted(bool isStarted)
        {
            InChannelStarted?.Invoke(this, isStarted);
        }

        private void RaiseOutChannelStarted(bool isStarted)
        {
            OutChannelStarted?.Invoke(this, isStarted);
        }
    }
}
