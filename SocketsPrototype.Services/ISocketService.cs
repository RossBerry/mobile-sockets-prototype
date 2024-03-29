﻿using System;
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

        bool IsListening { get; }
        bool IsSending { get; }

        int inPort { get; }

        Task Listen(int port);

        Task Send(string ip, int port);
    }
}
