using System;
using t4ccer.SharpProcessing;

namespace Debug
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            ProcessingRunner.Run(new NoiseFlowField(1, 10000, 20, 1.5f, 0.05, 0.01, 50, Math.PI * 2));
        }
    }
}
