using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Rebound
{
    public class WaterRoom
    {
        private Water water = null;
        private Point? input = null;
        private List<Point> outputs = new List<Point>();

        public WaterRoom(int width, int height)
        {
            water = new Water(width, height);
        }

        public WaterRoom(int width, int height, WaterRoom copyFrom) : this(width, height)
        {
            this.Input = copyFrom.Input;
            this.Outputs = copyFrom.Outputs;

            int copyWidth = Math.Min(water.MaskMatrix.GetLength(0), copyFrom.water.MaskMatrix.GetLength(0));
            int copyHeight = Math.Min(water.MaskMatrix.GetLength(1), copyFrom.water.MaskMatrix.GetLength(1));
            for(int x = 0; x < copyWidth; x++)
                for(int y = 0; y < copyHeight; y++)
                    water.MaskMatrix[x, y] = copyFrom.water.MaskMatrix[x, y];
        }

        public Water Water
        {
            get { return water; }
            set { water = value; }
        }

        public Point? Input
        {
            get { return input; }
            set { input = value; }
        }

        public List<Point> Outputs
        {
            get { return outputs; }
            set { outputs = value; }
        }
    }
}
