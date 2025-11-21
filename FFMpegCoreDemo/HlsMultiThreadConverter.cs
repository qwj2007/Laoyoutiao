using FFMpegCore;
using FFMpegCore.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFMpegCoreDemo
{
    /// <summary>
    /// 单文件多线程转换示例
    /// </summary>
    public class HlsMultiThreadConverter
    {
        /// <summary>
        /// 将单个视频文件转换为 HLS 格式，使用多线程优化
        /// </summary>
        /// <param name="inputPath">输入文件路径</param>
        /// <param name="outputDirectory">输出目录</param>
        /// <param name="segmentDuration">分片时长（秒）</param>
        /// <param name="threadCount">FFmpeg 线程数（传 0 表示让 FFmpeg 决定）</param>
        public static async Task<bool> ConvertSingleFileToHlsAsync(
            string inputPath,
            string outputDirectory,
            int segmentDuration = 10,
            int threadCount = 0)
        {
            try
            {
                // 检查输入文件是否存在，若不存在则直接返回 false
                if (!File.Exists(inputPath))
                {
                    Console.WriteLine($"文件不存在: {inputPath}");
                    return false;
                }

                // 确保输出目录存在（若不存在则创建）
                Directory.CreateDirectory(outputDirectory);

                // 构造输出 m3u8 文件路径，文件名取自输入文件名（不含扩展名）
                string fileName = Path.GetFileNameWithoutExtension(inputPath);
                string outputPath = Path.Combine(outputDirectory, $"{fileName}.m3u8");  
                // 为 FFmpeg 构造可识别的路径模式（使用正斜杠避免 Windows 路径分隔符问题）
                string segmentPattern = Path.Combine(outputDirectory, $"{fileName}_%03d.ts").Replace("\\", "/");
                // 构建 FFmpeg 参数：通过 FFMpegCore 的链式 API 构造输出参数并将其传给 ProcessAsynchronously 执行
                var arguments = FFMpegArguments
                    .FromFileInput(inputPath) // 指定输入文件
                    .OutputToFile(outputPath, overwrite: true, options => options
                        // 使用 x264 编码视频
                        .WithVideoCodec(VideoCodec.LibX264)
                        // 使用 AAC 编码音频
                        .WithAudioCodec(AudioCodec.Aac)
                        // 使用 VBR 质量等级（0-5）。这里使用 4 表示比较高的质量；
                        .WithVariableBitrate(4)
                        // 可选：缩放到预设的 HD 分辨率，避免输出分辨率过大
                        .WithVideoFilters(
                        filterOptions => filterOptions
                            .Scale(VideoSize.Hd)
                            )
                        // 优化 MP4 / HLS 开始播放速度
                        .WithFastStart()
                        // 自定义参数：设置编码预设，影响编码速度与质量折中
                        .WithCustomArgument("-preset fast")
                        // 设置线程数：当 threadCount 为 0 时，FFmpeg 会使用默认策略
                        .WithCustomArgument($"-threads {threadCount}")
                        // HLS 相关参数：分片时长、列表大小（0 表示无限）、分片文件名格式等
                        .WithCustomArgument($"-hls_time {segmentDuration}")
                        .WithCustomArgument("-hls_list_size 0")
                        // 指定分片文件输出路径模式（将 ts 文件写入指定的 segmentDir）
                        .WithCustomArgument($"-hls_segment_filename \"{segmentPattern}\"")
                        // 强制关键帧以保证分片边界处有关键帧（表达式根据需要调整）
                        .WithCustomArgument("-force_key_frames \"expr:gte(t,n_forced*2)\"")
                        // 指定输出格式为 HLS
                        .WithCustomArgument("-f hls"))
                      // 进度回调：打印已耗时（可按需扩展为百分比 + 已耗时）
                      .NotifyOnProgress(progress =>
                      {
                          Console.WriteLine($"转换进度：已耗时：{progress:hh\\:mm\\:ss}");
                      })
                    ;

                // 异步执行 FFmpeg 处理，注意可能会抛出异常（已在外层 try/catch 捕获）
                await arguments.ProcessAsynchronously();

                Console.WriteLine($"转换完成: {Path.GetFileName(inputPath)}");
                return true;
            }
            catch (Exception ex)
            {
                // 捕获并打印异常信息，返回 false 表示转换失败
                Console.WriteLine($"转换失败 {Path.GetFileName(inputPath)}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 自动检测最优线程数
        /// /// </summary>
        private static int GetOptimalThreadCount()
        {
            int processorCount = Environment.ProcessorCount;
            // 为系统保留一些资源，使用 75% 的 CPU 核心（至少 1 个）
            return Math.Max(1, (int)(processorCount * 0.75));
        }
    }
}
