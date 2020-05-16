using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ParallelExperiment
{
    class Program
    {
        private static int totalItem = 1000000;

        static void Main(string[] args)
        {
            Console.WriteLine("Create {0} object", totalItem);
            ExecuteTask();
        }

        static void ExecuteTask()
        {
            // 835
            Thread t1 = new Thread(async () =>
            {
                await ForLoopParallelAsync();
            });
            t1.Start();

            // 1146
            Thread t2 = new Thread(async () =>
            {
                await ForEachLoopParallelAsync();
            });
            t2.Start();

            // 1216
            Thread t3 = new Thread(async () =>
            {
                await ForEachLoopAsParallelAsync();
            });
            t3.Start();

            // 31
            Thread t4 = new Thread(async () =>
            {
                await ParallelForAsync();
            });
            t4.Start();

            // 41
            Thread t5 = new Thread(async () =>
            {
                await ParallelForEachAsync();
            });
            t5.Start();

            // 38
            Thread t6 = new Thread(async () =>
            {
                await ParallelForEachAsParallelAsync();
            });
            t6.Start();
        }


        static async Task ForLoopParallelAsync()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            var listObject = new List<object>();
            var listTask = new List<Task>();

            var objectLock = new object();

            for (int i = 0; i < totalItem; i++)
            {
                var task = Task.Run(() =>
                {
                    var newObject = new
                    {
                        Id = i,
                        Name = "Item " + i
                    };

                    lock (objectLock)
                    {
                        listObject.Add(newObject);
                    }
                });
                listTask.Add(task);
            }

            await Task.WhenAll(listTask);

            stopwatch.Stop();
            Console.WriteLine("ForLoopParallelAsync executed in {1} ms", totalItem, stopwatch.ElapsedMilliseconds);
            Console.ReadLine();
        }

        static async Task ForEachLoopParallelAsync()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            var listObject = new List<object>();
            var listTask = new List<Task>();

            var listInt = Enumerable.Range(0, totalItem);

            var objectLock = new object();

            listInt.ToList().ForEach(i =>
            {
                var task = Task.Run(() =>
                {
                    var newObject = new
                    {
                        Id = i,
                        Name = "Item " + i
                    };

                    lock (objectLock)
                    {
                        listObject.Add(newObject);
                    }
                });
                listTask.Add(task);
            });

            await Task.WhenAll(listTask);

            stopwatch.Stop();
            Console.WriteLine("ForEachLoopParallelAsync executed in {1} ms", totalItem, stopwatch.ElapsedMilliseconds);
            Console.ReadLine();
        }

        static async Task ForEachLoopAsParallelAsync()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            var listObject = new List<object>();
            var listTask = new List<Task>();

            var listInt = Enumerable.Range(0, totalItem);

            var objectLock = new object();

            listInt.AsParallel().ToList().ForEach(i =>
            {
                var task = Task.Run(() =>
                {
                    var newObject = new
                    {
                        Id = i,
                        Name = "Item " + i
                    };

                    lock (objectLock)
                    {
                        listObject.Add(newObject);
                    }
                });
                listTask.Add(task);
            });

            await Task.WhenAll(listTask);

            stopwatch.Stop();
            Console.WriteLine("ForEachLoopAsParallelAsync executed in {1} ms", totalItem, stopwatch.ElapsedMilliseconds);
            Console.ReadLine();
        }

        static async Task ParallelForAsync()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            var listObject = new List<object>();
            var listTask = new List<Task>();

            await Task.Run(() =>
            {
                var objectLock = new object();

                Parallel.For(0, totalItem, (id) =>
                {
                    var newObject = new
                    {
                        Id = id,
                        Name = "Item " + id
                    };

                    lock (objectLock)
                    {
                        listObject.Add(newObject);
                    }
                });
            });

            stopwatch.Stop();
            Console.WriteLine("ParallelForAsync executed in {1} ms", totalItem, stopwatch.ElapsedMilliseconds);
            Console.ReadLine();
        }

        static async Task ParallelForEachAsync()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            var listObject = new List<object>();
            var listTask = new List<Task>();

            var listInt = Enumerable.Range(0, totalItem);

            await Task.Run(() =>
            {
                var objectLock = new object();

                Parallel.ForEach<int>(listInt, (id) =>
                {
                    var newObject = new
                    {
                        Id = id,
                        Name = "Item " + id
                    };

                    lock (objectLock)
                    {
                        listObject.Add(newObject);
                    }
                });
            });

            stopwatch.Stop();
            Console.WriteLine("ParallelForEachAsync executed in {1} ms", totalItem, stopwatch.ElapsedMilliseconds);
            Console.ReadLine();
        }

        static async Task ParallelForEachAsParallelAsync()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            var listObject = new List<object>();
            var listTask = new List<Task>();

            var listInt = Enumerable.Range(0, totalItem);

            listInt = listInt.AsParallel().ToList();

            await Task.Run(() =>
            {
                var objectLock = new object();

                Parallel.ForEach<int>(listInt, (id) =>
                {
                    var newObject = new
                    {
                        Id = id,
                        Name = "Item " + id
                    };

                    lock (objectLock)
                    {
                        listObject.Add(newObject);
                    }
                });
            });

            stopwatch.Stop();
            Console.WriteLine("ParallelForEachAsParallelAsync executed in {1} ms", totalItem, stopwatch.ElapsedMilliseconds);
            Console.ReadLine();
        }
    }
}
