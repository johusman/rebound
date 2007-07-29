using System;
using System.Collections.Generic;
using System.Text;

namespace Rebound
{
    public class RMS
    {
        public static double calculateSingleRMS(double[] values)
        {
            double sum = 0.0;
            foreach(double value in values)
                sum += value * value;
            return Math.Sqrt(sum);
        }

        public static double[] calculateMultipleRMS(double[] values, int slots)
        {
            double[] results = new double[slots];
            int[] numSamples = new int[slots];
            double index = 0.0;
            double increment = ((double)slots) / values.Length;

            for(int i = 0; i < values.Length; i++)
            {
                results[(int)index] += values[i] * values[i];
                numSamples[(int)index]++;
                index += increment;
            }

            for(int i = 0; i < results.Length; i++)
                results[i] = Math.Sqrt(results[i] / numSamples[i]);

            return results;
        }

        
        
        private double[] results;
        private int[] numSamples;
        private double index;
        private double increment;

        public RMS(int numValues, int slots)
        {
            results = new double[slots];
            numSamples = new int[slots];
            index = 0.0;
            increment = ((double) slots)/numValues;
        }
        
        public void inputSample(double sample)
        {
            int intIndex = (int)index;
            results[intIndex] += sample * sample;
            numSamples[intIndex]++;

            if(((int)(index + increment)) > intIndex)
            {
                results[intIndex] = Math.Sqrt(results[intIndex] / numSamples[intIndex]);
            }

            index += increment;
        }

        public double[] getRMS()
        {
            double[] retval = new double[(int)index];
            Array.Copy(results, 0, retval, 0, (int)index);
            return retval;
        }

    }
}
