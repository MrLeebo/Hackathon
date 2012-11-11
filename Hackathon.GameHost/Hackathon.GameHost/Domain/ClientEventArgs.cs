using System;

namespace Hackathon.GameHost.Domain
{
    public class ClientEventArgs : EventArgs
    {
        public ClientData ClientData { get; set; }

        public ClientEventArgs(object data)
        {
            this.ClientData = data.ToObject<ClientData>();
        }
    }
}
