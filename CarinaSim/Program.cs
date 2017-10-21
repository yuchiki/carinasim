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
            //Console.WriteLine(program.Info);
            Console.SetOut(new StreamWriter("output.output"));
            var sim = new Simulator(program);
            sim.Initialize();

            int i = 0;
            foreach (var inst in program.program)
            {
                i += 4;
                if (Instruction.Convert(inst) == Instruction.Inst.hlt)
                    break;
            }
        

            Console.WriteLine(sim.FormattedStatus());

            var s = new Stopwatch();
            s.Start();
            while (sim.DoStep())
            {
                //    Console.WriteLine(sim.FormattedStatus());
                //  Console.Read();
            }
            s.Stop();
            Console.Error.WriteLine($"Total Instructions:{sim.DynamicCounts}");
            Console.Error.WriteLine($"time:{s.ElapsedMilliseconds / 1000.0}sec");
            Console.Error.WriteLine($"{(int)(1000.0 * sim.DynamicCounts /s.ElapsedMilliseconds):#,0}IPS");


        }
    }
}
