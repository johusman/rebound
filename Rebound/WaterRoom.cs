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
