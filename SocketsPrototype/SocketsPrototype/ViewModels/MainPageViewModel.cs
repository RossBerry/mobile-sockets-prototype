using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using SocketsPrototype.Models;
using SocketsPrototype.Services;
using Xamarin.Forms;

namespace SocketsPrototype.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {
        public ObservableCollection<DeviceModel> DetectedDevices { get; private set; } = new ObservableCollection<DeviceModel>();
        public ObservableCollection<DeviceModel> ConnectedDevices { get; set; } = new ObservableCollection<DeviceModel>();

        public ICommand StartInChannelCommand { get; private set; }
        public ICommand StartOutChannelCommand { get; private set; }
        public ICommand ScanForDevicesCommand { private set; get; }
        public ICommand ConnectToDeviceCommand { private set; get; }

        //public ICommand SendMessageCommand { get; private set; }

        private readonly ISocketService _socketService;

        public MainPageViewModel()
        {
            Title = "Scan";

            _socketService = DependencyService.Get<ISocketService>();
            //_socketService.DeviceDetected += OnDeviceDetected;
            _socketService.ErrorEvent += OnErrorEvent;
            _socketService.InfoEvent += OnInfoEvent;
            _socketService.InChannelStarted += OnInChannelStarted;
            _socketService.OutChannelStarted += OnOutChannelStarted;

            SetupButtonCommands();
        }

        private async Task CreateInChannel()
        {
            await _socketService.CreateInChannel();
        }

        private async Task CreateOutChannel()
        {
            await _socketService.CreateOutChannel();
        }

        private void ReadFromInChannel(string text)
        {
            _socketService.ReadFromInChannel(text);
        }

        private void SendToOutChannel(string text)
        {
            _socketService.SendToOutChannel(text);
        }

        //public void ScanForDevices()
        //{
        //    _rssdpService.BeginSearch();
        //}

        private string errorDetail;

        public string ErrorDetail
        {
            get { return errorDetail; }
            set { SetProperty(ref errorDetail, value); }
        }

        private string infoDetail;
        public string InfoDetail
        {
            get { return infoDetail; }
            set { SetProperty(ref infoDetail, value); }
        }

        private string sendMessageText;
        public string SendMessageText
        {
            get { return sendMessageText; }
            set { SetProperty(ref sendMessageText, value); }
        }

        private bool canStartServer = true;
        public bool CanStartServer
        {
            get { return canStartServer; }
            set { SetProperty(ref canStartServer, value); }
        }

        private bool canStartClient = true;
        public bool CanStartClient
        {
            get { return canStartClient; }
            set { SetProperty(ref canStartClient, value); }
        }

        private bool allowMessaging;
        public bool AllowMessaging
        {
            get { return allowMessaging; }
            set { SetProperty(ref allowMessaging, value); }
        }

        private void SetupButtonCommands()
        {
            StartInChannelCommand = new Command(async() => await CreateInChannel());
            StartOutChannelCommand = new Command(async() => await CreateOutChannel());
            //ScanForDevicesCommand = new Command(() => ScanForDevices());
            //SendMessageCommand = new Command(async () =>
            //{
            //    if (IsServer)
            //    {
            //        await SendToServer(SendMessageText);
            //    }
            //    else
            //    {
            //        SendToClients(SendMessageText);
            //    }
            //});
        }

        //private void OnDeviceDetected(object sender, DeviceModel model)
        //{
        //    if (DetectedDevices.Any(d => d.Id == model.Id || d.Name == model.Name))
        //        return;

        //    DetectedDevices.Add(model);
        //}

        //private void OnDeviceConnected(object sender, DeviceEventArgs e)
        //{
        //    if (e.Device != null)
        //    {
        //        var device = new DeviceModel
        //        {
        //            Id = e.Device.Id,
        //            Name = e.Device.Name
        //        };

        //        ConnectedDevices.Add(device);
        //    }
        //}

        private void OnErrorEvent(object sender, Exception e)
        {
            ErrorDetail = e.Message + "\r\n" + e.StackTrace;
        }

        private void OnInfoEvent(object sender, string info)
        {
            InfoDetail = info + "\r\n" + InfoDetail;
        }

        private void OnInChannelStarted(object sender, bool isStarted)
        {
            CanStartServer = !isStarted;
            AllowMessaging = isStarted;
        }

        private void OnOutChannelStarted(object sender, bool isStarted)
        {
            CanStartClient = !isStarted;
            AllowMessaging = isStarted;
        }
    }
}
