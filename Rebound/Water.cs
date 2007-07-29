using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Rebound
{
    public class Water
    {
        private long[][] mainMatrix;
        private long[][] copyMatrix;

        public Water(int width, int height)
        {
            InitializeMainMatrix(width, height);
            InitializeCopyMatrix(width, height);
        }

        public long[][] MainMatrix
        {
            get { return mainMatrix; }
        }

        private void InitializeMainMatrix(int width, int height)
        {
            mainMatrix = new long[height][];
            for(int i = 0; i < mainMatrix.Length; i++)
            {
                mainMatrix[i] = new long[width];
                for(int j = 0; j < mainMatrix[i].Length; j++)
                    mainMatrix[i][j] = 0;
            }
        }

        private void InitializeCopyMatrix(int width, int height)
        {
            copyMatrix = new long[height][];
            for(int i = 0; i < copyMatrix.Length; i++)
            {
                copyMatrix[i] = new long[width];
                for(int j = 0; j < copyMatrix[i].Length; j++)
                    copyMatrix[i][j] = 0;
            }
        }


        public void CalculateNextFrame()
        {
            long[][] tempMatrix = copyMatrix;
            copyMatrix = mainMatrix;
            mainMatrix = tempMatrix;

            for(int x = 1; x < mainMatrix.Length - 1; x++)
                for(int y = 1; y < mainMatrix[x].Length - 1; y++)
                    CalculateNextPixel(mainMatrix, copyMatrix, x, y);

            AddStatic();
        }

        private void AddStatic()
        {
            /*for(int x = 100; x < 120; x++)
                for(int y = 50; y < 80; y++)
                    mainMatrix[x][y] = 0;*/
        }

        private void CalculateNextPixel(long[][] targetMatrix, long[][] sourceMatrix, int x, int y)
        {
            targetMatrix[x][y] = -targetMatrix[x][y] + ((
                                sourceMatrix[x - 1][y]
                                + sourceMatrix[x + 1][y]
                                + sourceMatrix[x][y - 1]
                                + sourceMatrix[x][y + 1]) >> 1);

            targetMatrix[x][y] -= targetMatrix[x][y] >> 11;
        }

        public void Drop(int x, int y)
        {
            mainMatrix[y][x] = 32768 << 14;
        }

        public void Drop(int x, int y, long value)
        {
            mainMatrix[y][x] = value;
        }

        public long Read(int x, int y)
        {
            return mainMatrix[y][x];
        }
    }
}
