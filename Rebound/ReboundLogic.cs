using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;

namespace Rebound
{
    public interface ReboundCallback
    {
        void SignalStartedDirectCalculation();
        void SignalCompletedDirectCalculation();

        void SignalStartedImpulseCalculation();
        void SignalStartedConvolutionCalculation();
        void SignalEndedConvolutionCalculation();

        void SignalProgress(float percentDone);
        void SignalImpulseResponseProgress(float percentDone);
    }

    public interface ReboundInputStream
    {
        Int16[] ReadSamples();
    }

    public interface ReboundOutputStream
    {
        void WriteSamples(Int16[] values);
    }

    public class ReboundLogic
    {
        public void GenerateByBruteForce(ReboundInputStream inputStream, ReboundOutputStream outputStream, long inputLength, WaterRoom room, RMS rms, ReboundCallback callback)
        {
            callback.SignalStartedDirectCalculation();

            int samplesRead = 0;
            Int16[] outputs = new Int16[room.Outputs.Count];
            Int16[] inputs = null;
            while((inputs = inputStream.ReadSamples()).Length != 0)
            {
                samplesRead++;
                long averageInput = GetAverage(inputs);
                room.Water.Drop(room.Input.Value.X, room.Input.Value.Y, averageInput << 14);

                room.Water.CalculateNextFrame();

                for(int i = 0; i < room.Outputs.Count; i++)
                {
                    Point point = room.Outputs[i];
                    long output = room.Water.Read(point.X, point.Y) >> 14;
                    output = (output > Int16.MaxValue) ? Int16.MaxValue : output;
                    output = (output < Int16.MinValue) ? Int16.MinValue : output;
                    outputs[i] = (Int16) output;
                    
                    rms.inputSample(outputs[i] / 32768.0);
                }
                outputStream.WriteSamples(outputs);

                if((samplesRead & 511) == 0)
                {
                    callback.SignalProgress(((float) samplesRead)/inputLength);
                }
            }

            callback.SignalCompletedDirectCalculation();
        }

        public void GenerateByImpulseResponse(ReboundInputStream inputStream, ReboundOutputStream outputStream, long inputLength, WaterRoom room, RMS rms, ReboundCallback callback)
        {
            callback.SignalStartedImpulseCalculation();

            Dictionary<Point, long[]> responses = GenerateImpulseResponses(15000, room, rms, callback);

            callback.SignalStartedConvolutionCalculation();

            long[] inputChain = new long[inputLength];
            int samplesRead = 0;
            Int16[] outputs = new Int16[room.Outputs.Count];
            Int16[] inputs = null;
            while ((inputs = inputStream.ReadSamples()).Length != 0)
            {
                samplesRead++;
                long averageInput = GetAverage(inputs);
                inputChain[samplesRead-1] = averageInput;

                for (int i = 0; i < room.Outputs.Count; i++)
                {
                    Point point = room.Outputs[i]; 
                    long output = ConvolutePoint(inputChain, responses[point], samplesRead - 1) >> 14;
                    output = (output > Int16.MaxValue) ? Int16.MaxValue : output;
                    output = (output < Int16.MinValue) ? Int16.MinValue : output;
                    outputs[i] = (Int16)output;

                    rms.inputSample(outputs[i] / 32768.0);
                }
                outputStream.WriteSamples(outputs);

                if((samplesRead & 2047) == 0)
                {
                    callback.SignalProgress(((float) samplesRead)/inputLength);
                }
            }

            callback.SignalEndedConvolutionCalculation();
        }

        private static long GetAverage(Int16[] inputs)
        {
            long averageInput = 0;
            foreach (Int16 sample in inputs)
            {
                averageInput += (long)sample;
            }
            averageInput /= inputs.Length;
            return averageInput;
        }

        private long ConvolutePoint(long[] inputs, long[] response, int n)
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

        private Dictionary<Point, long[]> GenerateImpulseResponses(int length, WaterRoom room, RMS rms, ReboundCallback callback)
        {
            //rmsOutput.Init(length * 2);

            Dictionary<Point, long[]> responses = new Dictionary<Point, long[]>();
            foreach(Point point in room.Outputs)
                responses[point] = new long[length];

            room.Water.Drop(room.Input.Value.X, room.Input.Value.Y, (32767L) << 14);

            for(int i = 0; i < length; i++)
            {
                foreach(Point point in room.Outputs)
                {
                    long output = room.Water.Read(point.X, point.Y);
                    responses[point][i] = output >> 14;
                }

                room.Water.CalculateNextFrame();

                if((i & 127) == 0)
                {
                    callback.SignalImpulseResponseProgress(((float) i)/length);
                }
            }

            return responses;
        }

    }
}
