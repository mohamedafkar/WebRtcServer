using System;
using System.Collections.Generic;

namespace SRServer.Models
{
    public class MessageDto
    {

        public string User { get; set; }
        public string MsgText { get; set; }

        public string ConnectionId { get; set; }
    }
}
