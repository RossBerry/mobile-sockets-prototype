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
using Rssdp;

namespace SocketsPrototype.Services
{
    public class SocketService : ISocketService, IDisposable
    {
        public event EventHandler<Exception> ErrorEvent;
        public event EventHandler<string> InfoEvent;
        public event EventHandler<bool> PublishingStarted;
        public event EventHandler<bool> InChannelStarted;
        public event EventHandler<bool> OutChannelStarted;
        public event EventHandler<DeviceModel> DeviceDetected;

        private SsdpDeviceLocator _DeviceLocator;
        private List<DeviceModel> DetectedDevices = new List<DeviceModel>();

        public bool IsListening { get; private set; }
        public bool IsSending { get; private set; }

        private TcpSocketListener _inChannel = null;
        private TcpSocketClient _outChannel = null;
        private string outAddress = "192.168.50.179";
        private int inPort = 9000;
        private int outPort = 9000;

        public async Task CreateInChannel()
        {
            RaiseInfoEvent("Creating In Channel!");

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
            RaiseInfoEvent(string.Format("Listening on port {0}", inPort));
            RaiseInChannelStarted(true);
        }

        public async Task CreateOutChannel()
        {
            var r = new Random();
            
            RaiseInfoEvent(string.Format("Connecting to: {0}:{1}", outAddress, outPort));
            _outChannel = new TcpSocketClient();
            RaiseOutChannelStarted(true);

            await _outChannel.ConnectAsync(outAddress, outPort);
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

        public void PublishDevice()
        {
            string localAddress = GetLocalIPAddress();
            RaiseInfoEvent(string.Format("Publishing IP:{0}", localAddress));
            // As this is a sample, we are only setting the minimum required properties.
            var deviceDefinition = new SsdpRootDevice()
            {
                CacheLifetime = TimeSpan.FromMinutes(30), //How long SSDP clients can cache this info.
                Location = new Uri("http://mydevice/descriptiondocument.xml"), // Must point to the URL that serves your devices UPnP description document. 
                DeviceTypeNamespace = localAddress,
                DeviceType = "TDTDevice",
                FriendlyName = "Device Name",
                Manufacturer = "Me",
                ModelName = "MyCustomDevice",
                Uuid = Guid.NewGuid().ToString()
            };
        }

        public void BeginSearch()
        {
            RaiseInfoEvent("Scanning for devices!");

            _DeviceLocator = new SsdpDeviceLocator();

            // (Optional) Set the filter so we only see notifications for devices we care about 
            // (can be any search target value i.e device type, uuid value etc - any value that appears in the 
            // DiscoverdSsdpDevice.NotificationType property or that is used with the searchTarget parameter of the Search method).
            _DeviceLocator.NotificationFilter = "upnp:rootdevice";

            // Connect our event handler so we process devices as they are found
            _DeviceLocator.DeviceAvailable += deviceLocator_DeviceAvailable;

            // Enable listening for notifications (optional)
            _DeviceLocator.StartListeningForNotifications();

            // Perform a search so we don't have to wait for devices to broadcast notifications 
            // again to get any results right away (notifications are broadcast periodically).
            _DeviceLocator.SearchAsync();

            Console.ReadLine();
        }

        // Process each found device in the event handler
        async static void deviceLocator_DeviceAvailable(object sender, DeviceAvailableEventArgs e)
        {
            //Device data returned only contains basic device details and location of full device description.
            Console.WriteLine("Found " + e.DiscoveredDevice.Usn + " at " + e.DiscoveredDevice.DescriptionLocation.ToString());

            //Can retrieve the full device description easily though.
            SsdpDevice fullDevice = await e.DiscoveredDevice.GetDeviceInfo();
            //DetectedDevices.Add(new DeviceModel(fullDevice.DeviceTypeNamespace,
            //                              Guid.Parse(fullDevice.Uuid),
            //                              fullDevice.FriendlyName));
            //RaiseInfoEvent(string.Format("Detected {0}", fullDevice.DeviceTypeNamespace));
        }

        public void ReadFromInChannel(string text)
        {

        }

        public void SendToOutChannel(string text)
        {

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

        public void Dispose()
        {

        }

        //private void SetupEventHandling()
        //{
        //    //_adapter.ScanTimeoutElapsed += OnScanTimeoutElapsed;
        //    //_adapter.DeviceDisconnected += OnDeviceDisconnected;
        //    _adapter.DeviceConnected += OnDeviceConnected;
        //    //_adapter.DeviceConnectionLost += OnDeviceConnectionLost;
        //}

        //private void OnDeviceDetected(object sender, DeviceEventArgs e)
        //{
        //    if (!String.IsNullOrEmpty(e.Device?.Name))
        //    {
        //        try
        //        {
        //            DeviceDetected?.Invoke(this, e.Device);
        //        }
        //        catch (Exception ex)
        //        {
        //            RaiseErrorEvent(ex);
        //        }

        //    }
        //}
    }
}
