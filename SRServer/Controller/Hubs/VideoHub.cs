using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using SRServer.Models.Video;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SRServer.Controller.Hubs
{
    public class VideoHub : Hub
    {
        public ILogger<VideoHub> _logger { get; }

        public VideoHub(ILogger<VideoHub> logger)
        {
            _logger = logger;
        }

        

        //public override async Task OnConnectedAsync()
        //{
        //    _logger.LogInformation("Connected " + Context.ConnectionId);
        //    string x = "Welcome " + Context.ConnectionId;
        //    await Clients.Caller.SendAsync("broadcastConnectionId", x);
        //}

        //public override async Task OnDisconnectedAsync(Exception ex)
        //{
        //    _logger.LogInformation("Disconnecte " + Context.ConnectionId);
        //    await base.OnDisconnectedAsync(ex);
        //}


    }
}
