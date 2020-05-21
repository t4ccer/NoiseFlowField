using System;
using t4ccer.SharpProcessing;

namespace Debug
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            ProcessingRunner.Run(new NoiseFlowField(10, 100, 20, 0.5f, 0.05, 0.01, 50, Math.PI * 2));
        }
    }
}
