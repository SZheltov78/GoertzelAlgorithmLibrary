using GoertzelAlgorithmLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Un4seen.Bass;

namespace Example
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        static int stream;

        private void Form1_Load(object sender, EventArgs e)
        {
            Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, this.Handle);

            chart1.ChartAreas[0].AxisY.Maximum = 300;
            chart1.ChartAreas[0].AxisX.Minimum = 600;
            chart1.ChartAreas[0].AxisX.Maximum = 1700;
        }

        string LastSymbol = "";
        int Delay = 0;
        string NewSymbol = "";
        short[] RawData = new short[GoertzelAlgorithm.Length * 2];
        private void timer1_Tick(object sender, EventArgs e)
        {
            //read data
            int DataLen = Bass.BASS_ChannelGetData(stream, RawData, GoertzelAlgorithm.Length);
            if (DataLen < GoertzelAlgorithm.Length) return;

            //get DTMF symbol
            string Symbol = GoertzelAlgorithm.GetDTMFSymdol(RawData);
            label1.Text = Symbol;

            #region get DTMF tag
            if (Symbol != LastSymbol)
            {
                NewSymbol = Symbol;
            }
            LastSymbol = Symbol;


            if (NewSymbol != "")
            {
                textBox1.Text += NewSymbol;
                NewSymbol = "";
            }

            if (Symbol == "")
            {
                Delay++;
            }
            else
            {
                Delay = 0;
            }

            if (Delay == 5)
            {
                textBox1.Text += Environment.NewLine;
            }
            #endregion

            //update chart
            chart1.Series[0].Points.Clear();
            chart1.Series[0].Points.AddXY(697, GoertzelAlgorithm.M697);
            chart1.Series[0].Points.AddXY(770, GoertzelAlgorithm.M770);
            chart1.Series[0].Points.AddXY(941, GoertzelAlgorithm.M941);
            chart1.Series[0].Points.AddXY(1209, GoertzelAlgorithm.M1209);
            chart1.Series[0].Points.AddXY(1336, GoertzelAlgorithm.M1336);
            chart1.Series[0].Points.AddXY(1477, GoertzelAlgorithm.M1477);
            chart1.Series[0].Points.AddXY(1633, GoertzelAlgorithm.M1633);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            stream = Bass.BASS_StreamCreateFile("test.mp3", 0, 0, BASSFlag.BASS_DEFAULT);
            Bass.BASS_ChannelPlay(stream, false);
            timer1.Enabled = true;
        }
    }
}
