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

        void SignalProgress(int bytesProcessed);
        void SignalImpulseResponseProgress(int samplesGenerated);
    }

    public class ReboundLogic
    {
        public void GenerateByBruteForce(Stream inputStream, Stream outputStream, WaterRoom room, RMS rms, ReboundCallback callback)
        {
           callback.SignalStartedDirectCalculation();

            int bytesRead = 0;
            byte[] bytes = new byte[2];
            while(inputStream.Read(bytes, 0, 2) == 2)
            {
                bytesRead += 2;
                Int16 input = BitConverter.ToInt16(bytes, 0);
                room.Water.Drop(room.Input.Value.X, room.Input.Value.Y, ((long)input) << 14);

                room.Water.CalculateNextFrame();

                foreach(Point point in room.Outputs)
                {
                    long output = room.Water.Read(point.X, point.Y);
                    bytes = BitConverter.GetBytes((Int16)(output >> 14));
                    outputStream.Write(bytes, 0, 2);
                    rms.inputSample((output >> 14) / 32768.0);
                }

                if((bytesRead & 1023) == 0)
                {
                    callback.SignalProgress(bytesRead);
                }
            }

            callback.SignalCompletedDirectCalculation();
        }

        public void GenerateByImpulseResponse(Stream inputStream, Stream outputStream, WaterRoom room, RMS rms, ReboundCallback callback)
        {
            callback.SignalStartedImpulseCalculation();

            Dictionary<Point, long[]> responses = GenerateImpulseResponses(15000, room, rms, callback);

            callback.SignalStartedConvolutionCalculation();

            List<long> inputs = new List<long>();
            int bytesRead = 0;
            byte[] bytes = new byte[2];
            while(inputStream.Read(bytes, 0, 2) == 2)
            {
                bytesRead += 2;
                Int16 input = BitConverter.ToInt16(bytes, 0);
                inputs.Add(input);

                foreach(Point point in room.Outputs)
                {
                    long output = ConvolutePoint(inputs, responses[point], (bytesRead >> 1) - 1);
                    bytes = BitConverter.GetBytes((Int16)(output >> 14));
                    outputStream.Write(bytes, 0, 2);
                    rms.inputSample((output >> 14) / 32768.0);
                }

                if((bytesRead & 32767) == 0)
                {
                    callback.SignalProgress(bytesRead);
                }
            }

            callback.SignalEndedConvolutionCalculation();
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
                    //rmsOutput.AddSample((output >> 9) / 32768.0);
                }

                room.Water.CalculateNextFrame();

                if((i & 127) == 0)
                {
                    callback.SignalImpulseResponseProgress(i);
                }
            }

            return responses;
        }

    }
}
