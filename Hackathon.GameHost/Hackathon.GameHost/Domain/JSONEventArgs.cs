using System;
using PusherClientDotNet;

namespace Hackathon.GameHost.Domain
{
    public class JSONEventArgs: EventArgs
    {
        public string JSONData { get; set; }

        public JSONEventArgs(string data)
        {
            this.JSONData = data;
        }

        public JSONEventArgs(object json)
        {
            this.JSONData = Pusher.JSON.stringify(json);
        }
    }
}
