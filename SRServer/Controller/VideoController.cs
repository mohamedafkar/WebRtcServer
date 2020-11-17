using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using SRServer.Models.Video;

namespace SRServer.Controller.Hubs
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoController : ControllerBase
    {

        private readonly IHubContext<VideoHub> _hubContext;
        public ILogger<VideoController> _logger { get; }

        public VideoController(IHubContext<VideoHub> hubContext,
                                    ILogger<VideoController> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
            _logger.LogInformation("VideoController started");
        }
        
        

        [Route("onConnnect")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task OnConnnect([FromBody] Connnect massages)
        {
            _logger.LogInformation("OnConnnect called ");
             await _hubContext.Clients.All.SendAsync("onConnnect", massages.UserName);
            //return Ok(new ConnnectResponse
            //{;
            //    UserName = massages.UserName,
            //    IsConnected = true,
            //    AllUsers = ConnectedUser.Ids
            //});
        }

        [Route("sendStreamOne")]
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task SendStreamToOne([FromForm] UserStream userStream)
        {
            //var localStream = userStream.Stream.OpenReadStream();
            //var bytesinfile = new byte[userStream.Stream.Length];
            //localStream.Read(bytesinfile, 0, (int)userStream.Stream.Length);
            //HttpContext.Response..BinaryWrite(bytesinfile);

            
            FileStreamResult fileStreamResult = new FileStreamResult(userStream.Stream.OpenReadStream()
                , userStream.Stream.ContentType);

            
            await  _hubContext.Clients.All.SendAsync("sendStreamToOne", fileStreamResult.FileStream);

        }

        //[Route("getUsers")]
        //[HttpGet]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //public async Task<IActionResult> GetUsers()
        //{
        //    await _hubContext.Clients.All;
        //    return Ok();
        //}




    }
}
