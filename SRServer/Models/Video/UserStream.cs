using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SRServer.Models.Video
{
    public class UserStream
    {
        public string ConnectionId { get; set; }
        public IFormFile Stream { get; set; }
    }


}
