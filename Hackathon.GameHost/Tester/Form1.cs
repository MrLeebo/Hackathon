using System;
using System.Windows.Forms;
using PusherRESTDotNet;

namespace Tester
{
    public partial class Form1 : Form
    {
        private const string APP_ID = "31354";
        private const string APP_KEY = "6a4677394df2ac8f08d2";
        private const string SECRET = "0b981fba2c00f49d5dac";

        private IPusherProvider server;

        public Form1()
        {
            InitializeComponent();
        }

        private void ShutdownButton_Click(object sender, EventArgs e)
        {
            var request = new SimplePusherRequest("private-game-channel", "game:shutdown", "{}");
            server.Trigger(request);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.server = new PusherProvider(APP_ID, APP_KEY, SECRET);
        }
    }
}
