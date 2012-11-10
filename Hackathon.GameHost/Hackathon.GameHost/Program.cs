using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hackathon.GameHost
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new TumblrClient();
            var data = client.Tagged("lol");
            Console.WriteLine(data.ToString());
        }
    }
}
