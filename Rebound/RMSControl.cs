using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Rebound
{
    public partial class RMSControl : UserControl
    {
        private double[] data = null;

        private bool incremental;
        public bool Incremental
        {
            set { incremental = value; }
            get { return incremental; }
        }

        private double gain = 1.0;
        public double Gain
        {
            set { gain = value; }
            get { return gain; }
        }

        private RMS rms;

        public RMSControl()
        {
            InitializeComponent();
        }

        public void Init(int numSamples)
        {
            if(Incremental)
            {
                rms = new RMS(numSamples, this.Width);
            }
        }

        public void SetData(double[] values)
        {
            data = RMS.calculateMultipleRMS(values, this.Width);
        }

        public void AddSample(double value)
        {
            rms.inputSample(value);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if(this.DesignMode)
            {
                Random random = new Random(2343);
                data = new double[this.Width];
                for(int i = 0; i < data.Length; i++)
                    data[i] = random.NextDouble();
            }

            if(this.Incremental && !this.DesignMode)
                if(rms != null)
                    data = rms.getRMS();
                else
                    return;

            double middle = this.Height / 2;

            if(data != null)
                for(int i = 0; i < data.Length; i++)
                {
                    double dataPoint = data[i] * gain;
                    e.Graphics.DrawLine(new Pen(GetColor(dataPoint)), i, (float)(middle + dataPoint * middle), i, (float)(middle - dataPoint * middle));
                }
        }

        private Color GetColor(double data)
        {
            if(data > 1.0)
                data = 1.0;
            if(data < -1.0)
                data = -1.0;

            return Color.FromArgb((int) (255 * data), (int) (127 * (2 - data)), 0);
        }
    }
}
