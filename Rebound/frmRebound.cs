using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;

namespace Rebound
{
    public partial class frmRebound : Form
    {
        public Water water;

        public frmRebound()
        {
            InitializeComponent();
            water = new Water(water1.Width, water1.Height);
            water1.SetWater(water);
            water1.Input = new Point(30, 35);
            water1.Outputs.Add(new Point(160, 85));
            water1.Outputs.Add(new Point(150, 150));
        }

        private void btnProcess_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog();
            if(result != DialogResult.OK)
                return;
            result = saveFileDialog1.ShowDialog();
            if(result != DialogResult.OK)
                return;

            Process();
            /*ThreadStart job = new ThreadStart(Process);
            Thread thread = new Thread(job);
            thread.Start();*/
        }

        public void Process()
        {
            long fileSize = (new FileInfo(openFileDialog1.FileName)).Length;
            prgProcessing.Maximum = (int)fileSize;
            prgProcessing.Value = 0;

            using(Stream inputStream = openFileDialog1.OpenFile())
            {
                rmsOutput.Init((int)(fileSize));

                using(Stream outputStream = saveFileDialog1.OpenFile())
                {
                    if(rabDirect.Checked)
                        GenerateByBruteForce(inputStream, outputStream, fileSize);
                    else
                        GenerateByImpulseResponse(inputStream, outputStream, fileSize);
                }
            }
        }

        private void GenerateByBruteForce(Stream inputStream, Stream outputStream, long fileSize)
        {
            lblProcessing.Text = "Processing wave...";
            water1.Gain = 0;

            LoadInputRMS(fileSize, inputStream);

            int bytesRead = 0;
            byte[] bytes = new byte[2];
            while(inputStream.Read(bytes, 0, 2) == 2)
            {
                bytesRead += 2;
                Int16 input = BitConverter.ToInt16(bytes, 0);
                water.Drop(water1.Input.Value.X, water1.Input.Value.Y, ((long)input) << 14);

                water.CalculateNextFrame();

                foreach(Point point in water1.Outputs)
                {
                    long output = water.Read(point.X, point.Y);
                    bytes = BitConverter.GetBytes((Int16)(output >> 14));
                    outputStream.Write(bytes, 0, 2);
                    rmsOutput.AddSample((output >> 14) / 32768.0);
                }

                if((bytesRead & 1023) == 0)
                {
                    water1.Water_Update();
                    prgProcessing.Value = bytesRead;
                    water1.Refresh();
                    rmsOutput.Refresh();
                    Thread.Sleep(50);
                }
            }

            prgProcessing.Value = prgProcessing.Maximum;
            water1.Water_Update();
            water1.Refresh();
            rmsOutput.Refresh();
            lblProcessing.Text = "Done!";
        }

        private void GenerateByImpulseResponse(Stream inputStream, Stream outputStream, long fileSize)
        {
            lblProcessing.Text = "Generating impulse respones...";
            water1.Gain = 4;
            Refresh();

            Dictionary<Point, long[]> responses = GenerateImpulseResponses(15000);

            lblProcessing.Text = "Processing wave...";
            LoadInputRMS(fileSize, inputStream);
            Refresh();

            rmsOutput.Init((int)(fileSize));

            List<long> inputs = new List<long>();
            int bytesRead = 0;
            byte[] bytes = new byte[2];
            while(inputStream.Read(bytes, 0, 2) == 2)
            {
                bytesRead += 2;
                Int16 input = BitConverter.ToInt16(bytes, 0);
                inputs.Add(input);

                foreach(Point point in water1.Outputs)
                {
                    long output = ConvolutePoint(inputs, responses[point], (bytesRead >> 1) - 1);
                    bytes = BitConverter.GetBytes((Int16)(output >> 14));
                    outputStream.Write(bytes, 0, 2);
                    rmsOutput.AddSample((output >> 14) / 32768.0);
                }

                if((bytesRead & 32767) == 0)
                {
                    prgProcessing.Value = bytesRead;
                    rmsOutput.Refresh();
                    Thread.Sleep(50);
                }
            }

            prgProcessing.Value = prgProcessing.Maximum;
            water1.Water_Update();
            water1.Refresh();
            rmsOutput.Refresh();
            lblProcessing.Text = "Done!";
        }

        private long ConvolutePoint(List<long> inputs, long[] response, int n)
        {
            int lowerbound = n - response.Length + 1;
            if(lowerbound < 0)
                lowerbound = 0;
            int upperbound = n;

            long sum = 0;
            for(int i = lowerbound; i <= upperbound; i++)
                sum += response[n - i] * inputs[i];

            return sum;
        }

        private Dictionary<Point, long[]> GenerateImpulseResponses(int length)
        {
            rmsOutput.Init(length * 2);
            
            Dictionary<Point, long[]> responses = new Dictionary<Point,long[]>();
            foreach(Point point in water1.Outputs)
                responses[point] = new long[length];

            water.Drop(water1.Input.Value.X, water1.Input.Value.Y, (32767L) << 14);

            for(int i = 0; i < length; i++)
            {
                foreach(Point point in water1.Outputs)
                {
                    long output = water.Read(point.X, point.Y);
                    responses[point][i] = output >> 14;
                    rmsOutput.AddSample((output >> 9) / 32768.0);
                }

                water.CalculateNextFrame();

                if((i & 255) == 0)
                {
                    water1.Water_Update();
                    water1.Refresh();
                    rmsOutput.Refresh();
                    Thread.Sleep(50);
                }
            }

            return responses;
        }

        private void LoadInputRMS(long fileSize, Stream inputStream)
        {
            double[] data = new double[fileSize / 2];
            byte[] value = new byte[2];
            int pos = 0;
            while(inputStream.Read(value, 0, 2) == 2)
            {
                data[pos] = BitConverter.ToInt16(value, 0) / 32768.0;
                pos++;
            }

            rmsInput.SetData(data);
            inputStream.Seek(0, SeekOrigin.Begin);
            rmsInput.Refresh();
        }

        private void frmRebound_ResizeEnd(object sender, EventArgs e)
        {
            water = new Water(water1.Width, water1.Height);
            water1.SetWater(water);
        }
    }
}