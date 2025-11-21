// See https://aka.ms/new-console-template for more information
using FFMpegCore;
using FFMpegCore.Enums;
using FFMpegCore.Exceptions;
using FFMpegCoreDemo;

#region
//Console.WriteLine("Hello, World!");
//try
//{
//    GlobalFFOptions.Configure(new FFOptions { BinaryFolder = @"path\to\ffmpeg\bin" });
//    string inputMp4Path = @"D:\input.mp4";//源mp4文件路径
//    string outputDir = @"D:\HlsOutput";//输出目录（m3u8+ts切片会保存在这里）
//    string outputM3u8Path = Path.Combine(outputDir, "output.m3u8");//最终m3u8路径
//    if (!Directory.Exists(outputDir))
//    {
//        Directory.CreateDirectory(outputDir);
//        Console.WriteLine($"已经创建输出目录：{outputDir}");
//    }
//    if (!File.Exists(inputMp4Path))
//    {
//        throw new FileNotFoundException("源 MP4 文件不存在", inputMp4Path);
//    }
//    // 4. 使用 FFMpegCore 构建 HLS 切割命令
//    // 核心参数说明：
//    // - Codec: 视频用 H.264（兼容所有设备），音频用 AAC（HLS 标准音频格式）
//    // - HlsTime: 每个 TS 切片的时长（单位：秒，推荐 5-10 秒，平衡加载速度和切换流畅度）
//    // - HlsListSize: 0 表示 M3U8 包含所有切片（默认只包含最近 5 个）
//    // - HlsSegmentFilename: TS 切片的命名格式（%03d 表示 3 位数字自增，如 001.ts、002.ts）
//    // 将 .WithHlsTime(10) 替换为添加自定义参数实现 HLS 切片时长设置
//    // FFMpegCore 没有 WithHlsTime 方法，需要用 WithCustomArgument 添加 -hls_time 参数
//    var conversionResult = FFMpegArguments
//                    .FromFileInput(inputMp4Path) // 输入 MP4 文件
//                    .OutputToFile(
//                        outputM3u8Path,          // 输出 M3U8 文件
//                        false,                   // 是否覆盖已存在的文件（false = 覆盖）
//                        options => options
//                            .WithVideoCodec(VideoCodec.LibX264) // 视频编码器（H.264）
//                            .WithAudioCodec(AudioCodec.Aac)    // 音频编码器（AAC）

//                            .WithCustomArgument("-hls_time 10")                  // 每个切片 10 秒
//                            .WithCustomArgument("-hls_list_size 0")              // M3U8 包含所有切片
//                            .WithCustomArgument($"-hls_segment_filename \"{Path.Combine(outputDir, "segment_%03d.ts")}\"") // TS 切片命名
//                            .WithConstantRateFactor(23)        // 视频质量（0-51，越小质量越好，23 为默认推荐）
//                            .WithCustomArgument("-ar 44100") // 音频采样率（44.1kHz 标准）
//                    )
//                    // .Cancellable() // 已移除
//                    .NotifyOnProgress(progress => // 进度回调（可选）
//                    {
//                        Console.WriteLine($"转换进度：{progress:F1}% | 已耗时：{progress.Duration:hh\\:mm\\:ss}");
//                    })
//                    .ProcessSynchronously(); // 替换 Execute 为 ProcessSynchronously

//    // 5. 处理执行结果
//    if (conversionResult)
//    {
//        Console.WriteLine($"转换成功！");
//        Console.WriteLine($"M3U8 文件路径：{outputM3u8Path}");
//        Console.WriteLine($"TS 切片目录：{outputDir}");
//    }
//}
//catch (FileNotFoundException ex)
//{
//    Console.ForegroundColor = ConsoleColor.Red;
//    Console.WriteLine($"错误：{ex.Message}");
//}
//catch (FFMpegException ex)
//{
//    Console.ForegroundColor = ConsoleColor.Red;
//    Console.WriteLine($"FFmpeg 执行错误：{ex.Message}");
//    Console.WriteLine("请检查 FFmpeg 是否安装正确，或输入文件是否损坏");
//}
//catch (Exception ex)
//{
//    Console.ForegroundColor = ConsoleColor.Red;
//    Console.WriteLine($"未知错误：{ex.Message}");
//}
//finally
//{
//    Console.ResetColor();
//    Console.WriteLine("\n程序结束");
//}
#endregion


    // 配置 FFmpeg 路径（如果需要）
    GlobalFFOptions.Configure(new FFOptions { BinaryFolder = @"E:\studyCode\Laoyoutiao\FFMpegCoreDemo\ffmpeg\bin" });

    // 示例1：转换单个文件
   // await ConvertSingleFileExample();

    // 示例2：批量转换多个文件
    //await ConvertMultipleFilesExample();

    // 示例3：高级转换选项
    await AdvancedConversionExample();

//转换单个文件
static async Task ConvertSingleFileExample()
{
    Console.WriteLine("=== 单文件转换示例 ===");

    string inputFile = @"E:\video\sample.mp4";
    string outputDir = @"E:\video\hls_output\sample";

    // 自动使用最优线程数
    int optimalThreads = Environment.ProcessorCount/4;
    bool success = await HlsMultiThreadConverter.ConvertSingleFileToHlsAsync(
        inputFile, outputDir, segmentDuration: 10, threadCount: optimalThreads);

    Console.WriteLine($"单文件转换结果: {(success ? "成功" : "失败")}");
}

//批量文件转换示例
static async Task ConvertMultipleFilesExample()
{
    Console.WriteLine("\n=== 批量文件转换示例 ===");

    var inputFiles = new List<string>
        {
            @"E:\video\sample.mp4",
            @"E:\video\test.mp4",           
        };

    string baseOutputDir = @"E:\video\hls_batch_output";

    bool success = await BatchHlsConverter.ConvertMultipleFilesToHlsAsync(
        inputFiles, baseOutputDir, segmentDuration: 10, maxConcurrentTasks: 2);

    Console.WriteLine($"批量转换结果: {(success ? "成功" : "失败")}");
}

//高级转换
static async Task AdvancedConversionExample()
{
    Console.WriteLine("\n=== 高级转换示例 ===");

    string inputFile = @"E:\video\test.mp4";
    string outputDir = @"E:\video\hls_advanced\test";

    var options = new HlsConversionOptions
    {
        SegmentDuration = 15,
        VideoBitrate = 3000,
        VideoThreads = 6,
        AudioThreads = 2,
        UseMultiThreadedEncoding = true
    };

    bool success = await AdvancedHlsConverter.ConvertToHlsAdvancedAsync(
        inputFile, outputDir, options);

    Console.WriteLine($"高级转换结果: {(success ? "成功" : "失败")}");
}
