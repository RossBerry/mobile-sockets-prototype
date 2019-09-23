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
        public event EventHandler<bool> MessageEntryStarted;

        public ICommand ListenCommand { get; private set; }
        public ICommand SendCommand { get; private set; }
        public ICommand MessageEntryCommand { get; private set; }


        private readonly ISocketService _socketService;

        public MainPageViewModel()
        {
            Title = "Scan";

            _socketService = DependencyService.Get<ISocketService>();
            _socketService.ErrorEvent += OnErrorEvent;
            _socketService.InfoEvent += OnInfoEvent;
            _socketService.InChannelStarted += OnInChannelStarted;
            _socketService.OutChannelStarted += OnOutChannelStarted;
            MessageEntryStarted += OnMessageEntryStarted;

            SetupButtonCommands();
        }

        private void StartMessageEntry()
        {
            RaiseMessageEntryStarted(true);
        }

        private async Task Listen()
        {
            await _socketService.Listen(_socketService.inPort);
        }

        private async Task Send()
        {
            await _socketService.Send(sendMessageText, sendAddress, 9000);
        }

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

        private string sendAddress;
        public string SendAddress
        {
            get { return sendAddress; }
            set { SetProperty(ref sendAddress, value); }
        }

        private int sendPort;
        public int SendPort
        {
            get { return sendPort; }
            set { SetProperty(ref sendPort, value); }
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

        private bool messageEntry;
        public bool MessageEntry
        {
            get { return messageEntry; }
            set { SetProperty(ref messageEntry, value); }
        }

        private void SetupButtonCommands()
        {
            ListenCommand = new Command(async() => await Listen());
            MessageEntryCommand = new Command(() => StartMessageEntry());
            SendCommand = new Command(async() => await Send());
        }

        private void OnErrorEvent(object sender, Exception e)
        {
            ErrorDetail = e.Message + "\r\n" + e.StackTrace;
        }

        private void OnInfoEvent(object sender, string info)
        {
            InfoDetail += "\r\n" + info;
        }

        private void OnInChannelStarted(object sender, bool isStarted)
        {
            CanStartServer = !isStarted;
        }

        private void OnOutChannelStarted(object sender, bool isStarted)
        {
            CanStartClient = !isStarted;
            MessageEntry = isStarted;
        }

        private void OnMessageEntryStarted(object sender, bool isStarted)
        {
            MessageEntry = isStarted;
        }

        private void RaiseMessageEntryStarted(bool isStarted)
        {
            MessageEntryStarted?.Invoke(this, isStarted);
        }
    }
}
