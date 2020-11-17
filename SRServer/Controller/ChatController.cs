using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SRServer.Models;
using Microsoft.AspNetCore.SignalR;

namespace SRServer.Controller
{

    //    [Route("api/chat")]
    //    [ApiController]
    //    public class ChatController : ControllerBase
    //    {
    //        private readonly IHubContext<ChatHub> _hubContext;

    //        public ChatController(IHubContext<ChatHub> hubContext)
    //        {
    //            _hubContext = hubContext;
    //        }

    //        [Route("send")]  
    //        [HttpPost]
    //        public IActionResult SendRequest([FromBody] MessageDto msg)
    //        {
    //            _hubContext.Clients.All.SendAsync("ReceiveOne", msg.User, msg.MsgText);
    //            return Ok();
    //        }
    //        ///HubCallerContext
    //        [Route("sendone")]
    //        [HttpPost]
    //        public IActionResult SendToOne([FromBody] MessageDto msg)
    //        {

    //            _hubContext.Clients.Client(msg.ConnectionId).SendAsync("SendOne", msg.User, msg.MsgText);
    //            return Ok();
    //        }


    //        [Route("sendstreamtoone")]
    //        [HttpPost]
    //        public IActionResult SendStreamToOne([FromBody] Streaming streaming)
    //        {

    //            _hubContext.Clients.Client(streaming.ConnectionId).SendAsync("SendStreamToOne", streaming.MyStream);
    //            return Ok();
    //        }




    //}
    
}
