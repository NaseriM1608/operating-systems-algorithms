using System;
using System.Collections.Generic;

namespace Deadlock_Detection_Algorithm
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int[] Available = new int[] { 6, 3, 5, 4 };
            int[] Work = Available;
            int[,] Allocation = new int[,]
            {
                { 2, 0, 2, 1 },
                { 0, 1, 1, 1 },
                { 4, 1, 0, 2 },
                { 1, 0, 0, 1 },
                { 1, 1, 0, 0 },
                { 1, 0, 1, 1 }
            };
            int[,] Request = new int[,]
            {
                { 9, 5, 5, 5 },
                { 2, 3, 3, 3 },
                { 7, 5, 4, 4 },
                { 3, 3, 3, 2 },
                { 5, 2, 2, 1 },
                { 4, 4, 4, 4 }
            };
            
            List<int> Marked = new List<int>();
            bool Mark = true;
            
            int rows = 0;
            int iterations = 0;
            int maxIterations = Request.GetLength(0);

            for (int i = 0; i < Request.GetLength(0); i++)
            {
                Mark = true;
                for (int j = 0; j < Request.GetLength(1); j++)
                {
                    if (Request[i, j] != 0)
                    {
                        Mark = false;
                        break;
                    }
                }
                if (Mark) Marked.Add(i);
            }
            while (Marked.Count != Request.GetLength(0))
            {
                Mark = true;
                if (rows == Request.GetLength(0))
                {
                    rows = 0;
                    iterations++;
                }
                if (iterations > maxIterations)
                {
                    Console.WriteLine("Deadlock Detected.");
                    return;
                }
                if (!Marked.Contains(rows))
                {
                    for (int j = 0; j < Request.GetLength(1); j++)
                    {
                        if (Request[rows, j] > Work[j])
                        {
                            Mark = false;
                            break;
                        }
                    }
                    if (Mark)
                    {
                        for (int j = 0; j < Request.GetLength(1); j++)
                        {
                            Work[j] += Allocation[rows, j];
                        }
                        Marked.Add(rows);
                    }
                }
                rows++;
            }
            Console.WriteLine("No Deadlocks");
            Console.WriteLine("\n");
            Console.Write("Available: ");
            foreach (var item in Work)
            {
                Console.Write($"{item} ");
            }
            Console.WriteLine("\n");
        }
    }
}
