using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SRServer.Models.Video
{
    public class ConnnectResponse
    {
        public string ConnectionId { get; set; }
        public string UserName { get; set; }
        public bool IsConnected { get; set; }

        public List<string> AllUsers { get; set; }

        public ConnnectResponse()
        {
            AllUsers = new List<string>();
        }


    }
}
