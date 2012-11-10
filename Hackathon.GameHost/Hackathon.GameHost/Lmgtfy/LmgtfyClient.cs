using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace Hackathon.GameHost.Lmgtfy
{
    public class LmgtfyClient
    {
        public LmgtfyResponse Load()
        {
            HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create("http://live.lmgtfy.com/recent.json");
            HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();
            // Get the stream associated with the response.
            Stream receiveStream = response.GetResponseStream();

            // Pipes the stream to a higher level stream reader with the required encoding format. 
            StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
            string result = readStream.ReadToEnd();

            LmgtfyObject[] yay = JsonConvert.DeserializeObject<LmgtfyObject[]>(result);


            return new LmgtfyResponse() { Response = yay };
        }
    }
}
