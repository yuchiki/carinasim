using System;
using System.IO;
using System.Diagnostics;

namespace CarinaSim
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            var program = new MipsProgram("../../mandelbrot.o");
            Console.SetOut(new StreamWriter("output.output"));
            var s = new Stopwatch();
            s.Start();
            var sim = new Simulator(program);
            sim.Initialize();
            while (sim.DoStep())
            {
            }
            s.Stop();
            Console.Error.WriteLine($"Total Instructions:{sim.DynamicCounts}");
            Console.Error.WriteLine($"time:{s.ElapsedMilliseconds / 1000.0}sec");
            Console.Error.WriteLine($"{(int)(1000.0 * sim.DynamicCounts /s.ElapsedMilliseconds):#,0}IPS");


        }
    }
}
