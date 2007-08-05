using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Diagnostics;

namespace Rebound
{
    public partial class frmRebound : Form, ReboundCallback
    {
        public volatile WaterRoom room;

        public frmRebound()
        {
            InitializeComponent();
            room = new WaterRoom(water1.Width, water1.Height);
            room.Input = new Point(30, 35);
            room.Outputs.Add(new Point(160, 85));
            room.Outputs.Add(new Point(150, 150));
            water1.SetWaterRoom(room);
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
        }

        public void Process()
        {
            long fileSize = (new FileInfo(openFileDialog1.FileName)).Length;
            prgProcessing.Maximum = (int)fileSize;
            prgProcessing.Value = 0;

            using(Stream inputStream = openFileDialog1.OpenFile())
            {
                LoadInputRMS(fileSize, inputStream);
                rmsOutput.Init((int)(fileSize));
            }

            ParameterizedThreadStart job = new ParameterizedThreadStart(BackgroundThreadEntryPoint);
            Thread thread = new Thread(job);
            thread.IsBackground = true;
            ThreadParameters parameters = new ThreadParameters();
            parameters.room = this.room;
            parameters.rms = this.rmsOutput.Rms;
            Debug.Assert(parameters.room != null);
            Debug.Assert(parameters.room.Input != null);
            Debug.Assert(parameters.room.Outputs != null);
            Debug.Assert(parameters.room.Outputs.Count > 0);
            thread.Start(parameters);
        }

        public void BackgroundThreadEntryPoint(object param)
        {
            ThreadParameters parameters = (ThreadParameters)param;

            using(Stream inputStream = openFileDialog1.OpenFile())
            {
                using(Stream outputStream = saveFileDialog1.OpenFile())
                {
                    ReboundLogic logic = new ReboundLogic();

                    if(rabDirect.Checked)
                        logic.GenerateByBruteForce(inputStream, outputStream, parameters.room, parameters.rms, new ReboundCallbackAdapter(this));
                    else
                        logic.GenerateByImpulseResponse(inputStream, outputStream, parameters.room, parameters.rms, new ReboundCallbackAdapter(this));
                }
            }
        }

        private void DisableUserInput()
        {
            btnProcess.Enabled = false;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
        }

        private void EnableUserInput()
        {
            btnProcess.Enabled = true;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MaximizeBox = true;
        }

        public void SignalCompletedDirectCalculation()
        {
            prgProcessing.Value = prgProcessing.Maximum;
            water1.Water_Update();
            water1.Refresh();
            rmsOutput.Refresh();
            lblProcessing.Text = "Done!";
            EnableUserInput();
        }

        public void SignalProgress(int bytesRead)
        {
            if(this.WindowState != FormWindowState.Minimized)
            {
                water1.Water_Update();
                prgProcessing.Value = bytesRead;
                water1.Refresh();
                rmsOutput.Refresh();
            }
        }

        public void SignalImpulseResponseProgress(int samplesGenerated)
        {
            if(this.WindowState != FormWindowState.Minimized)
            {
                water1.Water_Update();
                //            prgProcessing.Value = samplesGenerated;
                water1.Refresh();
                rmsOutput.Refresh();
            }
        }

        public void SignalStartedDirectCalculation()
        {
            lblProcessing.Text = "Processing wave...";
            water1.Gain = 0;
            DisableUserInput();
        }

        public void SignalEndedConvolutionCalculation()
        {
            prgProcessing.Value = prgProcessing.Maximum;
            water1.Water_Update();
            water1.Refresh();
            rmsOutput.Refresh();
            lblProcessing.Text = "Done!";
            EnableUserInput();
        }

        public void SignalStartedConvolutionCalculation()
        {
            lblProcessing.Text = "Processing wave...";
            Refresh();
        }

        public void SignalStartedImpulseCalculation()
        {
            lblProcessing.Text = "Generating impulse responses...";
            water1.Gain = 3;
            Refresh();
            DisableUserInput();
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
            if(btnProcess.Enabled)
            {
                WaterRoom newRoom = new WaterRoom(water1.Width, water1.Height);
                newRoom.Input = room.Input;
                newRoom.Outputs = room.Outputs;
                room = newRoom;
                water1.SetWaterRoom(room);
                water1.Refresh();
            }
        }
    }

    class ThreadParameters
    {
        public WaterRoom room;
        public RMS rms;
    }

    class ReboundCallbackAdapter : ReboundCallback
    {
        private delegate void noArgumentCall();
        private delegate void intArgumentCall(int arg);
        frmRebound form;

        public ReboundCallbackAdapter(frmRebound form)
        {
            this.form = form;
        }
        
        #region ReboundCallback Members

        public void SignalStartedDirectCalculation()
        {
            form.Invoke(new noArgumentCall(form.SignalStartedDirectCalculation));
        }

        public void SignalCompletedDirectCalculation()
        {
            form.Invoke(new noArgumentCall(form.SignalCompletedDirectCalculation));
        }

        public void SignalStartedImpulseCalculation()
        {
            form.Invoke(new noArgumentCall(form.SignalStartedImpulseCalculation));
        }

        public void SignalStartedConvolutionCalculation()
        {
            form.Invoke(new noArgumentCall(form.SignalStartedConvolutionCalculation));
        }

        public void SignalEndedConvolutionCalculation()
        {
            form.Invoke(new noArgumentCall(form.SignalEndedConvolutionCalculation));
        }

        public void SignalProgress(int bytesProcessed)
        {
            form.Invoke(new intArgumentCall(form.SignalProgress), bytesProcessed);
        }

        public void SignalImpulseResponseProgress(int samplesGenerated)
        {
            form.Invoke(new intArgumentCall(form.SignalImpulseResponseProgress), samplesGenerated);
        }

        #endregion
    }
}