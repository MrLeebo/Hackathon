using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Hackathon.GameHost.Lmgtfy
{
    public class LmgtfyResponse
    {
        public LmgtfyObject[] Response { get; set; }
    }

    public class LmgtfyObject
    {
        public string url { get; set; }
        public string q { get; set; }
    }
}
