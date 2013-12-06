using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Linpack.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            const int iterations = 1000;
            var cpuCount = Environment.ProcessorCount;
            var results = new List<double>();

            Parallel.For(0, iterations, y =>
            {
                var innerresult = 0.0;
                Parallel.For(0, cpuCount, x =>
                {
                    var l = new Linpack();
                    var r = l.run_benchmark();
                    innerresult += r.MFlops;
                });
                results.Add(innerresult);
            });

           Debug.WriteLine(results.Average());
        }
    }
}
