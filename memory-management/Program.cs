using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;

namespace Fixed_Partitioning
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await Console.Out.WriteAsync("Enter total memory size: ");
            int memory_size = Convert.ToInt32(Console.ReadLine());
            await Console.Out.WriteAsync("Enter number of memory blocks: ");
            int blocks_num = Convert.ToInt32(Console.ReadLine());
            await Console.Out.WriteAsync("Enter number of processes: ");
            int processes_num = Convert.ToInt32(Console.ReadLine());
            Memory Memory = new Memory(blocks_num, memory_size);
            Queue<Process> process_queue = new Queue<Process>();
        //سایز دادن به هر بلاک در حافظه
        Block_Sizing:
            int blocks_sum = 0;
            for (int i = 0; i < blocks_num - 1; i++)
            {
                await Console.Out.WriteAsync($"Enter the size memory block number {i}: ");

                int block_size = Convert.ToInt32(Console.ReadLine());
                blocks_sum += block_size;
                if (blocks_sum > Memory.Total_Size)
                {
                    await Console.Out.WriteAsync("Total Size Exceeded.");
                    Thread.Sleep(800);
                    await Console.Out.WriteAsync("Try Again.");
                    Memory._blocks.Clear();
                    Thread.Sleep(800);
                    Console.Clear();
                    goto Block_Sizing;
                }
                Memory._blocks.Add
                (
                    new Memory_Block(block_size)
                );
            }
            Memory._blocks.Add
            (
                new Memory_Block(Memory.Total_Size - Memory._blocks.Sum(b => b.Size))
            );
            //سایز دادن به هر پروسس
            for (int i = 0; i < processes_num; i++)
            {
                await Console.Out.WriteAsync($"Enter the size of P{i}: ");
                int process_size = Convert.ToInt32(Console.ReadLine());
                process_queue.Enqueue
                (
                    new Process(process_size)
                );
            }
            await SingleProcessQueue(process_queue, Memory);
            await MultipleProcessQueues(process_queue, Memory);
        }
        static async Task SingleProcessQueue(Queue<Process> queue, Memory Memory)
        {
            Stopwatch stopwatch = new Stopwatch();
            List<Process> processes = new List<Process>();
            Queue<Process> process_queue = new Queue<Process>(queue);
            processes = process_queue.ToList();
            double total_service_time = 0;
            //تخصیص حافظه به هر پروسس به روش Best-Fit
            stopwatch.Start();
            while (process_queue.Count > 0)
            {
                int min_sub = int.MaxValue;
                Process process = process_queue.Dequeue();
                Memory_Block selected_block = null;
                foreach (Memory_Block block in Memory._blocks)
                {
                    if (block.Size >= process.Size && block.Size - process.Size < min_sub)
                    {
                        min_sub = block.Size - process.Size;
                        selected_block = block;
                    }
                }
                //در صورتی که بلاک مناسبی برای پروسس پیدا شود
                if (selected_block != null)
                {
                    if (selected_block.Occupied)
                    {
                        await Task.Delay((int)selected_block.Process.ResidencyTime);
                    }
                    total_service_time += selected_block.Size * process.ResidencyTime;
                    selected_block.Process = process;
                    selected_block.Occupied = true;
                    process.Allocated = true;
                }
            }
            stopwatch.Stop();
            double timeElapsed = stopwatch.Elapsed.TotalMilliseconds;
            List<Process> allocated_processes = processes.Where(p => p.Allocated).ToList();
            int total_unoccupied_memory_space = Memory._blocks.Where(b => !b.Occupied).ToList().Sum(b => b.Size);
            double throughput = Math.Round(allocated_processes.Count / timeElapsed, 2);
            double utilization = Math.Round((double)total_service_time / (Memory.Total_Size * timeElapsed) * 100, 2);
            await Console.Out.WriteLineAsync("\nSingle Process Queue:");
            await Console.Out.WriteLineAsync($"Utilization: {utilization}%");
            await Console.Out.WriteLineAsync($"Throughput: {throughput * 100} Processes per Second");
        }
        static async Task MultipleProcessQueues(Queue<Process> process_queue, Memory Memory)
        {
            Stopwatch stopwatch = new Stopwatch();
            List<Process> processes = new List<Process>();
            processes = process_queue.ToList();
            Queue<Process>[] process_queue_list = new Queue<Process>[Memory._blocks.Count];
            for (int i = 0; i < process_queue_list.Length; i++)
            {
                process_queue_list[i] = new Queue<Process>();
            }
            foreach (Process process in process_queue)
            {
                int min_sub = int.MaxValue;
                int selected_queue_index = 0;
                Process selected_process = null;
                foreach (Memory_Block block in Memory._blocks)
                {
                    if (block.Size >= process.Size && block.Size - process.Size < min_sub)
                    {
                        min_sub = block.Size - process.Size;
                        selected_queue_index = block.ID;
                        selected_process = process;
                    }
                }
                if (selected_process != null)
                    process_queue_list[selected_queue_index].Enqueue(process);
            }
            double total_service_time = 0;
            for (int i = 0; i < Memory.Blocks_Num; i++)
            {
                total_service_time += Memory._blocks[i].Size * process_queue_list[i].ToList().Sum(p => p.ResidencyTime);
            }
            stopwatch.Start();
            Task[] blockTasks = Memory._blocks.Select((block, i) => ProcessBlockAsync(block, process_queue_list[i])).ToArray();
            await Task.WhenAll(blockTasks);
            stopwatch.Stop();
            double timeElapsed = stopwatch.Elapsed.TotalMilliseconds;
            double utilization = Math.Round(total_service_time / (Memory.Total_Size * timeElapsed) * 100, 2);
            double throughput = Math.Round(processes.Where(p => p.Allocated).ToList().Count / timeElapsed, 2);
            await Console.Out.WriteLineAsync("\nMultiple Process Queues:");
            await Console.Out.WriteLineAsync($"Utilization: {utilization}%");
            await Console.Out.WriteLineAsync($"Throughput: {throughput * 100} Processes per Second");
        }
        private static async Task ProcessBlockAsync(Memory_Block block, Queue<Process> queue)
        {
            while (queue.Count > 0)
            {
                Process process = queue.Dequeue();
                block.Process = process;
                block.Occupied = true;
                await Task.Delay((int)process.ResidencyTime);
                block.Process = null;
                block.Occupied = false;
            }
        }
    }
}
