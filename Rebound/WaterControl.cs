using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace Rebound
{
    public partial class WaterControl : UserControl
    {
        private WaterRoom room = null;
        private Bitmap bitmap = null;
        public enum RenderingModeEnum { WaveHeight = 0, WaveDelta = 1 };
        private RenderingModeEnum renderingMode = RenderingModeEnum.WaveHeight;
        private int gain = 0;

        public WaterControl()
        {
            InitializeComponent();
        }

        public WaterControl(WaterRoom room) : this()
        {
            SetWaterRoom(room);
        }

        public RenderingModeEnum RenderingMode
        {
            get { return renderingMode; }
            set { renderingMode = value; }
        }

        public int Gain
        {
            get { return gain; }
            set { gain = value; }
        }

        public void SetWaterRoom(WaterRoom room)
        {
            this.room = room;
            bitmap = new Bitmap(this.Width, this.Height, PixelFormat.Format32bppRgb);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if(this.DesignMode)
                e.Graphics.FillEllipse(Brushes.Aquamarine, new Rectangle(0, 0, this.Width, this.Height));
            if(bitmap != null)
            {
                e.Graphics.DrawImage(bitmap, 0, 0);
                DrawInputsAndOutputs(e.Graphics);
            }
        }

        public void Water_Update()
        {
            RenderBitmap(room.Water.MainMatrix);
            this.Refresh();
        }

        private unsafe void RenderBitmap(long[][] matrix)
        {
            BitmapData bmd = bitmap.LockBits(new Rectangle(0, 0, this.Width, this.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, bitmap.PixelFormat);

            for(int y = 1; y < matrix.Length - 1; y++)
            {
                byte* row = (byte*)bmd.Scan0 + (y * bmd.Stride);

                for(int x = 1; x < matrix[y].Length - 1; x++)
                {
                    long value = (renderingMode == RenderingModeEnum.WaveHeight) ? matrix[y][x] : matrix[y - 1][x] - matrix[y + 1][x];
                    int idx = x << 2;
                    Color color = CalcColor(value);
                    row[idx] = color.B;
                    row[idx + 1] = color.R;
                    row[idx + 2] = color.G;
                }
            }

            bitmap.UnlockBits(bmd);
        }

        private Color CalcColor(long value)
        {
            value = value >> (12 - gain);

            if(value > 32767)
                value = 32767;
            if(value < -32767)
                value = -32767;

            int r = 0;
            if(value > 0)
                r = (int)(value >> 7);
            int g = r;
            int b = (int)(value >> 8) + 128;

            return Color.FromArgb(r, g, b);
        }

        private void DrawInputsAndOutputs(Graphics graphics)
        {
            if(room.Input != null)
                graphics.DrawEllipse(new Pen(Color.FromArgb(255, 255, 0), 1), room.Input.Value.X - 2, room.Input.Value.Y - 2, 5, 5);
            foreach(Point output in room.Outputs)
                graphics.DrawEllipse(new Pen(Color.FromArgb(255, 0, 255), 1), output.X - 2, output.Y - 2, 5, 5);
        }
    }
}
