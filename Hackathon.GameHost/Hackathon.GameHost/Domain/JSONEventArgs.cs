using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hackathon.GameHost.Domain
{
    public class JSONEventArgs: EventArgs
    {
        public string JSONData { get; set; }

        public JSONEventArgs(string data)
        {
            this.JSONData = data;
        }

    }
}
