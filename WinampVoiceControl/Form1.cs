using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Forms;

namespace WinampVoiceControl
{
    public partial class Form1 : Form
    {
        private CustomVoiceRecog voice = new CustomVoiceRecog();


        public Form1()
        {
            InitializeComponent();
        }
        
        private void Form1_Load_1(object sender, EventArgs e)
        {
            this.Hide();
            Process[] processes = Process.GetProcesses();

            foreach (Process process in processes)
            {
                if (process.ProcessName == "winamp")
                {
                    voice.winampHandle = process.MainWindowHandle;
                    break;
            }
            }
            voice.AddCommandWord("pause");
            voice.AddCommandWord("play");
            voice.AddCommandWord("next");
            voice.AddCommandWord("previous");
            voice.AddCommandWord("last");
            voice.AddCommandWord("stop");
            voice.AddCommandWord("volume up ten");
            voice.AddCommandWord("volume up fifty");
            voice.AddCommandWord("volume up max");
            voice.AddCommandWord("volume down ten");
            voice.AddCommandWord("volume down fifty");
            voice.AddCommandWord("volume down max");
            voice.AddCommandWord("winamp listen");
            voice.AddCommandWord("winamp ignore");
            voice.InitVoiceHandler();
            voice.StartListening();

        }

        }


}
