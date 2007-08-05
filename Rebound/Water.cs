using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Rebound
{
    public class Water
    {
        private long[,] mainMatrix;
        private long[,] copyMatrix;
        private bool[,] maskMatrix;

        public Water(int width, int height)
        {
            mainMatrix = new long[width, height];
            copyMatrix = new long[width, height];
            maskMatrix = new bool[width, height];

            /*for(int x = 1; x < width - 1; x++)
                for(int y = 1; y < height - 1; y++)
                    maskMatrix[x, y] = (Math.Sqrt(Math.Pow(x-width/2,2) + Math.Pow(y-height/2,2)) > height/2);*/
            
            /*for(int x = 1; x < width - 1; x++)
                for(int y = 1; y < height - 1; y++)
                    maskMatrix[x, y] = (x == width/2 && (y - width/2)*(y - width/2) > 20); */
        }

        public long[,] MainMatrix
        {
            get { return mainMatrix; }
        }

        public bool[,] MaskMatrix
        {
            get { return maskMatrix; }
        }

        public void CalculateNextFrame()
        {
            long[,] tempMatrix = copyMatrix;
            copyMatrix = mainMatrix;
            mainMatrix = tempMatrix;

            for(int x = 1; x < mainMatrix.GetLength(0) - 1; x++)
                for(int y = 1; y < mainMatrix.GetLength(1) - 1; y++)
                    CalculateNextPixel(mainMatrix, copyMatrix, x, y);

            ApplyMask();
        }

        private void ApplyMask()
        {
            for(int x = 0; x < mainMatrix.GetLength(0); x++)
                for(int y = 0; y < mainMatrix.GetLength(1); y++)
                    if(maskMatrix[x, y])
                        mainMatrix[x, y] = 0;
        }

        private void CalculateNextPixel(long[,] targetMatrix, long[,] sourceMatrix, int x, int y)
        {
            targetMatrix[x,y] = -targetMatrix[x,y] + ((
                                sourceMatrix[x - 1,y]
                                + sourceMatrix[x + 1,y]
                                + sourceMatrix[x,y - 1]
                                + sourceMatrix[x,y + 1]) >> 1);

            targetMatrix[x,y] -= targetMatrix[x,y] >> 11;
        }

        public void Drop(int x, int y)
        {
            mainMatrix[x,y] = 32768 << 14;
        }

        public void Drop(int x, int y, long value)
        {
            mainMatrix[x,y] = value;
        }

        public long Read(int x, int y)
        {
            return mainMatrix[x,y];
        }
    }
}
