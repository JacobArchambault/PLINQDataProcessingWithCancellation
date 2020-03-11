using System;
using System.Linq;
using System.Threading;
using static System.Console;
using static System.Threading.Tasks.Task;
using static System.Linq.Enumerable;

namespace PLINQDataProcessingWithCancellation
{
    class Program
    {
        static readonly CancellationTokenSource cancelToken = new CancellationTokenSource();

        static void Main()
        {
            do
            {
                WriteLine("Start any key to start processing");
                ReadKey();

                WriteLine("Processing");
                Factory.StartNew(() => ProcessIntData());
                Write("Enter Q to quit: ");
                string answer = ReadLine();

                // Does user want to quit?
                if (answer.Equals("Q", StringComparison.OrdinalIgnoreCase))
                {
                    cancelToken.Cancel();
                    break;
                }
            } while (true);
            ReadLine();
        }

        static void ProcessIntData()
        {
            // Get a very large array of integers. 
            int[] source = Range(1, 10_000_000).ToArray();
            try
            {
                // Find the numbers where num % 3 == 0 is true, returned
                // in descending order. 
                int[] modThreeIsZero = (from num in source
                                        .AsParallel()
                                        .WithCancellation(cancelToken.Token)
                                        where num % 3 == 0
                                        orderby num descending
                                        select num)
                                        .ToArray();
                WriteLine();
                WriteLine($"Found {modThreeIsZero.Count()} numbers that match query!");
            }
            catch (OperationCanceledException ex)
            {
                WriteLine(ex.Message);
            }
        }
    }
}