using System;
using System.Collections.Generic;

namespace Bankers_Algorithm
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int[] Available = new int[] { 0, 3, 2 };
            int[,] Allocation = new int[,]
            {
                { 2, 2, 0 },
                { 3, 2, 0 },
                { 1, 1, 3 },
                { 0, 0, 2 }
            };
            int[,] Max = new int[,]
            {
                { 4, 5, 3 },
                { 3, 2, 3 },
                { 1, 2, 3 },
                { 1, 0, 2 }
            };
            int[,] Need = new int[Allocation.GetLength(0), Allocation.GetLength(1)];
            List<int> Sequence = new List<int>();
            bool canProceed = true;

            //به دست آوردن منابع مورد نیاز هر پروسس
            for (int i = 0; i < Allocation.GetLength(0); i++)
            {
                for (int j = 0; j < Allocation.GetLength(1); j++)
                {
                    Need[i, j] = Max[i, j] - Allocation[i, j];
                }
            }

            //مقایسه منابع موجود با منابع مورد نیاز هر پروسس
            int rows = 0;
            int iterations = 0; 
            int maxIterations = Need.GetLength(0);
            while (Sequence.Count != Need.GetLength(0))
            {
                canProceed = true;
                if (rows == Need.GetLength(0))
                {
                    rows = 0;
                    iterations++;
                }
                if (iterations > maxIterations)
                {
                    Console.WriteLine("System is not in a safe state");
                    return;
                }
                if (!Sequence.Contains(rows))
                {
                    for (int j = 0; j < Need.GetLength(1); j++)
                    {
                        if (Need[rows, j] > Available[j])
                        {
                            canProceed = false;
                            break;
                        }
                    }
                    if (canProceed)
                    {
                        for (int j = 0; j < Need.GetLength(1); j++)
                        {
                            Available[j] += Allocation[rows, j];
                        }
                        Sequence.Add(rows);
                    }
                }
                rows++;
            }
            Console.WriteLine("System is safe.");
            Console.WriteLine("Safe sequence: P" + string.Join(" -> P", Sequence));
            Console.WriteLine("\n");
            Console.Write("Total Resources: ");
            foreach (var item in Available)
            {
                Console.Write($"{item} ");
            }
            Console.WriteLine("\n");
        }
    }
}
