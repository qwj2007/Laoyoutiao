using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFMpegCoreDemo
{
    /// <summary>
    /// 批量转换多个 MP4 文件为 HLS 格式
    /// </summary>
    public class BatchHlsConverter
    {
        /// <summary>
        /// 批量转换多个 MP4 文件为 HLS 格式
        /// </summary>
        public static async Task<bool> ConvertMultipleFilesToHlsAsync(
            List<string> inputFiles,
            string baseOutputDirectory,
            int segmentDuration = 10,
            int maxConcurrentTasks = 0)
        {
            try
            {
                if (maxConcurrentTasks <= 0)
                {
                    // 自动设置最大并发任务数（CPU核心数的50%）
                    maxConcurrentTasks = Math.Max(1, Environment.ProcessorCount / 2);
                }

                Console.WriteLine($"开始批量转换，最大并发任务数: {maxConcurrentTasks}");
                Console.WriteLine($"总文件数: {inputFiles.Count}");

                // 创建信号量限制并发数
                var semaphore = new System.Threading.SemaphoreSlim(maxConcurrentTasks);
                var tasks = new List<Task<bool>>();

                foreach (var inputFile in inputFiles)
                {
                    // 等待信号量
                    await semaphore.WaitAsync();

                    var task = Task.Run(async () =>
                    {
                        try
                        {
                            string fileName = Path.GetFileNameWithoutExtension(inputFile);
                            string outputDir = Path.Combine(baseOutputDirectory, fileName);
                           
                            // 自动设置最优线程数
                            int threadCount = GetOptimalThreadCount();

                            return await HlsMultiThreadConverter.ConvertSingleFileToHlsAsync(
                                inputFile, outputDir, segmentDuration, threadCount);
                        }
                        finally
                        {
                            // 释放信号量
                            semaphore.Release();
                        }
                    });

                    tasks.Add(task);
                }

                // 等待所有任务完成
                var results = await Task.WhenAll(tasks);
                int successCount = results.Count(r => r);

                Console.WriteLine($"批量转换完成！成功: {successCount}/{inputFiles.Count}");
                return successCount == inputFiles.Count;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"批量转换失败: {ex.Message}");
                return false;
            }
        }

        private static int GetOptimalThreadCount()
        {
            int processorCount = Environment.ProcessorCount;
            return Math.Max(1, (int)(processorCount * 0.75));
        }
    }
}
