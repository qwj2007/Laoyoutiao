using FFMpegCore;
using FFMpegCore.Enums;

namespace FFMpegCoreDemo
{
    /// <summary>
    /// 高级 HLS 转换类，封装了将任意输入文件转换为 HLS（.m3u8 + .ts 切片）的逻辑。
    /// 该类使用 FFMpegCore 库，提供可配置的编码、分片时长与多线程优化选项。
    /// </summary>
    public class AdvancedHlsConverter
    {
        /// <summary>
        /// 高级 HLS 转换，支持更多多线程优化选项
        /// 异步执行：创建输出目录 -> 构建输出参数 -> 启动异步转换 -> 返回是否成功
        /// </summary>
        /// <param name="inputPath">输入文件路径（音视频文件）</param>
        /// <param name="outputDirectory">输出目录（会在该目录下生成 playlist.m3u8 与若干 segment_XXX.ts）</param>
        /// <param name="options">HLS 转换的可配置选项</param>
        /// <returns>异步返回是否转换成功（true 表示成功，false 表示失败）</returns>
        public static async Task<bool> ConvertToHlsAdvancedAsync(
            string inputPath,
            string outputDirectory,
            HlsConversionOptions options)
        {
            try
            {
                // 确保目标输出目录存在；若不存在则创建该目录
                Directory.CreateDirectory(outputDirectory);

                // 将输出主播放列表固定命名为 playlist.m3u8，放在输出目录下
                string outputPath = Path.Combine(outputDirectory, "playlist.m3u8");

                // 使用 FFMpegArguments 构建复杂的多线程和 HLS 参数
                // FromFileInput：指定输入文件
                // OutputToFile：指定输出文件与覆盖选项，并通过回调设置具体的输出参数
                var argumentBuilder = FFMpegArguments
                    .FromFileInput(inputPath)
                    .OutputToFile(outputPath, overwrite: true, outputOptions =>
                        // 将具体的输出选项构建逻辑委托给 BuildAdvancedOutputOptions 方法
                        BuildAdvancedOutputOptions(outputOptions, options, outputDirectory))
                    .NotifyOnProgress(progress => // 进度回调（可选），在控制台输出百分比与已耗时
                    {
                        // progress 为 double（示例），Duration 为 TimeSpan；格式化输出便于监控
                        Console.WriteLine($"转换进度：已耗时：{progress:hh\\:mm\\:ss}");
                    })
                    ;

                // 异步启动 FFMpeg 进程，等待完成
                await argumentBuilder.ProcessAsynchronously();

                // 完成后输出提示
                Console.WriteLine("高级转换完成！");
                return true;
            }
            catch (Exception ex)
            {
                // 捕获所有异常并在控制台输出错误信息；返回 false 表示失败
                Console.WriteLine($"高级转换失败: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 构建高级的输出参数：包括视频/音频编码、HLS 特殊参数、性能与多线程优化等
        /// 该方法不会启动转换，只会对传入的 options 对象追加 ffmpeg 命令参数
        /// </summary>
        /// <param name="options">FFMpegArgumentOptions 实例，用于链式配置 ffmpeg 参数</param>
        /// <param name="conversionOptions">用户自定义的 HLS 转换选项</param>
        private static void BuildAdvancedOutputOptions(
            FFMpegArgumentOptions options,
            HlsConversionOptions conversionOptions,
            string outputDirectory
            )
        {

            // ---------------- 视频编码设置 ----------------
            // 使用 x264 编码（libx264），并设置视频码率与帧率
            options.WithVideoCodec(VideoCodec.LibX264)
                   .WithVideoBitrate(conversionOptions.VideoBitrate) // 设置目标视频码率（kbps）
                   .WithFramerate(conversionOptions.FrameRate) // 设置帧率（fps），注意使用 WithFramerate 方法
                   // 自定义线程参数用于视频编码线程数（仅作为额外 ffmpeg 参数）
                   .WithCustomArgument($"-threads:v {conversionOptions.VideoThreads}");

            // ---------------- 音频编码设置 ----------------
            // 使用 AAC 音频编码，设置音频质量或比特率；同时可设置音频线程数
            options.WithAudioCodec(AudioCodec.Aac)
                   .WithAudioBitrate(AudioQuality.Normal) // 使用预设的音频质量
                   .WithCustomArgument($"-threads:a {conversionOptions.AudioThreads}");

            // ---------------- HLS 特有设置 ----------------
            // -hls_time 指定每个分片的时长（秒）
            // -hls_list_size 0 表示播放列表中包含所有分片，不移除旧片段
            // -hls_segment_filename 指定分片文件的命名模板
            // -f hls 强制输出格式为 HLS
            string fileName = "playlist";
            string segmentPattern = Path.Combine(outputDirectory, $"{fileName}_%03d.ts").Replace("\\", "/");
            options.WithCustomArgument($"-hls_time {conversionOptions.SegmentDuration}")
                   .WithCustomArgument("-hls_list_size 0")
                   .WithCustomArgument($"-hls_segment_filename \"{segmentPattern}\"")
                   .WithCustomArgument("-f hls");

            // ---------------- 性能优化参数 ----------------
            // preset fast：x264 预设，平衡速度与压缩率
            // tune zerolatency：用于实时编码以降低延迟（例如直播场景）
            // movflags +faststart：使得 mp4 在网络上更快开始播放（对 mp4 有用，但对 HLS 也是常见优化项）
            options.WithCustomArgument("-preset fast")
                   .WithCustomArgument("-tune zerolatency")
                   .WithCustomArgument("-movflags +faststart");

            // ---------------- 可选的多线程 x264 参数 ----------------
            // 如果启用了 UseMultiThreadedEncoding，则向 x264 传入额外的线程参数（frame-threads / slice-threads）
            // 这些参数可根据 CPU 核心数调整，避免设置过高以致于效率反而下降
            if (conversionOptions.UseMultiThreadedEncoding)
            {
                options.WithCustomArgument("-x264-params frame-threads=8:slice-threads=8");
            }
        }
    }

    /// <summary>
    /// HLS 转换配置选项
    /// 该类包含用户可配置的属性，控制分片时长、视频码率、帧率以及线程相关的参数
    /// </summary>
    public class HlsConversionOptions
    {
        /// <summary>
        /// 每个 HLS 分片的时长（秒），默认 10 秒
        /// 较短分片能更快切换清晰度，但会增大索引与请求数量
        /// </summary>
        public int SegmentDuration { get; set; } = 10;

        /// <summary>
        /// 目标视频码率（kbps），默认 2500 kbps
        /// 根据输出分辨率与质量需求调整该值
        /// </summary>
        public int VideoBitrate { get; set; } = 2500;

        /// <summary>
        /// 输出帧率（fps），默认 30fps
        /// 如果输入帧率不同，FFmpeg 会做相应的帧率转换
        /// </summary>
        public int FrameRate { get; set; } = 30;

        /// <summary>
        /// 视频编码时使用的线程数（仅作为 -threads:v 参数传递），默认 4
        /// 注意：部分编码器自行管理线程，过高的线程数不一定带来性能提升
        /// </summary>
        public int VideoThreads { get; set; } = 4;

        /// <summary>
        /// 音频编码时使用的线程数（仅作为 -threads:a 参数传递），默认 2
        /// 一般音频编码开销较小，通常使用较少线程即可
        /// </summary>
        public int AudioThreads { get; set; } = 2;

        /// <summary>
        /// 是否使用额外的多线程 x264 参数（-x264-params），默认启用
        /// 启用后会向 x264 传递 frame-threads 与 slice-threads，可能提升多核 CPU 的编码效率
        /// </summary>
        public bool UseMultiThreadedEncoding { get; set; } = true;
    }
}

