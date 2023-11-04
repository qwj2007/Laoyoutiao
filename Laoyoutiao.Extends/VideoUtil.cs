//using System.ComponentModel;
//using System.Diagnostics;

//namespace Laoyoutiao.Extends
//{

//    /// <summary>
//    /// 视频转换工具
//    /// </summary>
//    public class VideoUtil
//    {
//        #region 相关属性
//        private static string imgsize = "400*300";     //视频截图大小        
//        //wwwroot
//        public static string fileDir
//        {
//            get
//            {
//                return ServiceProviderInstance.wwwrootpath + @"\ffmpeg\ffmpeg.exe";
//            }

//        }
//        //文件保存路径
//        public static string TargetFilepath
//        {
//            get
//            {

//                return AppDomain.CurrentDomain.BaseDirectory;// + $"/Resource/Video/";

//            }
//        }


//        /// <summary>
//        /// 图片路径
//        /// </summary>
//        public static string DestImage
//        {
//            get; set;
//        }
//        /// <summary>
//        /// 视频长度
//        /// </summary>
//        public static string VideoLength { get; set; }
//        #endregion

//        /// <summary>
//        /// 转换图片
//        /// </summary>
//        /// <param name="videoPath"></param>
//        /// <param name="imageSavePath"></param>
//        /// <param name="width"></param>
//        /// <param name="height"></param>
//        public static void GetImage(string videoPath, string imageSavePath, int width, int height)
//        {
//            try
//            {
//                var fi = new FileInfo(imageSavePath);
//                if (fi.Exists)
//                {
//                    fi.IsReadOnly = false;
//                    fi.Delete();
//                }
//                var tool = fileDir;

//                var command = " -i \"" + videoPath + "\" -y -f image2 -t 0.001 -s " + width + "x" + height + " \"" + imageSavePath + "\"";
//                Console.WriteLine(tool + command);
//                Process _p;
//                using (_p = new Process())
//                {
//                    RedirectRun.RedirectExcuteProcess(_p, tool, command, (s, e) => WriteLog(e.Data));
//                }
//            }
//            catch (Exception err)
//            {
//                Console.WriteLine(err.ToString());
//            }
//        }
//        /// <summary>
//        /// 转换为FLV文件
//        /// </summary>
//        /// <param name="sourceFile"></param>
//        /// <param name="targetFile"></param>
//        public static void ConvertFlv(string sourceFile, string targetFile)
//        {
//            var sf = new FileInfo(sourceFile);
//            var tf = new FileInfo(targetFile);
//            try
//            {
//                if (sf.Extension.ToLower() == ".flv")
//                {
//                    sf.CopyTo(tf.FullName);
//                    return;
//                }
//                ConvertFlvByFFmpeg(sf.FullName, tf.FullName);
//            }
//            catch (Exception err)
//            {
//                Console.Write(err.ToString());
//            }
//        }
//        private static bool ConvertFlvByFFmpeg(string vFileName, string exportName)
//        {
//            var tool = fileDir;
//            //var command = " -i \"" + vFileName + "\" -y -ab 32 -ar 22050 -b 800000 -s  640*480 \"" + exportName + "\""; //Flv格式     
//            var command = " -i \"" + vFileName + "\" -vcodec copy -acodec copy \"" + exportName + "\""; //Flv格式     

//            Process _p;
//            using (_p = new Process())
//            {
//                RedirectRun.RedirectExcuteProcess(_p, tool, command, (s, e) => WriteLog(e.Data));
//            }
//            return true;
//        }
//        /// <summary>
//        ///     功能：转换视频文件为M3U8             
//        /// </summary>
//        /// <param name="sourceFlv">源文件路径</param>
//        /// <param name="m3U8Name">转换后的路径</param>
//        /// <returns></returns>
//        public static bool ConvertM3U8(string sourceFlv, string m3u8path, string m3U8Name)
//        {
//            try
//            {
//                //文件名称,生成M3U8文件
//                // var tool = Environment.CurrentDirectory + @"\ffmpeg\ffmpeg.exe";
//                var tool = fileDir;
//                var command = " -i \"" + sourceFlv + "\"  -hls_time 30 -c:v libx264 -hls_list_size 0 -c:a aac  -strict -2 -f hls " + m3U8Name + ".m3u8";
//                Process _p;
//                using (_p = new Process())
//                {
//                    _p.StartInfo.WorkingDirectory = m3u8path;
//                    RedirectRun.RedirectExcuteProcess(_p, tool, command, (s, e) => WriteLog(e.Data));
//                }

//                return true;
//            }
//            catch
//            {
//                return false;
//            }
//        }

//        /// <summary>
//        /// 写日志文件
//        /// </summary>
//        /// <param name="data"></param>
//        public static void WriteLog(String data)
//        {
//            if (!String.IsNullOrEmpty(data))
//            {
//                //Program.MyFrm.SetTextMessage(data);
//                Console.WriteLine(data);
//            }
//        }
//        /// <summary>
//        /// 转换成m3u8格式
//        /// </summary>
//        /// <param name="sourcePath"></param>
//        /// <param name="filename"></param>
//        /// <returns></returns>
//        public static string ConvertToM3U8(string sourcePath, string filename)
//        {
//            try
//            {
//                //创建的文件夹
//                var saveDir = DateTime.Now.ToString("yyyyMMddHHmmss");
//                var videoname = filename.Split('.')[0];
//                string chDir = "/Resource/Video/" + saveDir + "/";
//                var d = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + "Resource/Video/");
//                //转换成m3u8格式
//                d = new DirectoryInfo(TargetFilepath + "\\Resource\\Video\\" + saveDir);
//                if (!d.Exists) d.Create();
//                var tm3u8 = new FileInfo(d.FullName + "\\" + videoname + ".m3u8");

//                if (!tm3u8.Exists)
//                {
//                    bool result = VideoUtil.ConvertM3U8(TargetFilepath + "\\" + sourcePath, d.FullName, videoname);
//                    if (result)
//                    {
//                        return chDir + videoname + ".m3u8";
//                    }
//                }
//                return "";
//            }
//            catch (Exception ex)
//            {
//                return "";
//            }


//        }

//        #region 使用FFmpeg进行格式转换
//        /// <summary>
//        /// 返回枚举类型的描述信息
//        /// </summary>
//        /// <param name="myEnum"></param>
//        /// <returns></returns>
//        private static string GetDiscription(System.Enum myEnum)
//        {

//            System.Reflection.FieldInfo fieldInfo = myEnum.GetType().GetField(myEnum.ToString());
//            object[] attrs = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), true);
//            if (attrs != null && attrs.Length > 0)
//            {
//                DescriptionAttribute desc = attrs[0] as DescriptionAttribute;
//                if (desc != null)
//                {
//                    return desc.Description.ToLower();
//                }
//            }
//            return myEnum.ToString();
//        }
//        /// <summary>
//        /// 运行格式转换
//        /// </summary>
//        /// <param name="sourceFile">要转换文件绝对路径</param>
//        /// <param name="destPath">转换结果存储的相对路径</param>
//        /// <param name="videotype">要转换成的文件类型</param>
//        /// <param name="createImage">是否生成截图</param>
//        /// <returns>
//        /// 执行成功返回空，否则返回错误信息
//        /// </returns>
//        public static string ConvertVideo(string sourceFile, string filename, VideoType videotype, bool createImage = false, bool getDuration = false)
//        {
//            string sourceFilePath = TargetFilepath + "\\" + sourceFile;
//            //取得ffmpeg.exe的物理路径
//            string ffmpeg = fileDir;
//            if (!File.Exists(ffmpeg))
//            {
//                return "找不到格式转换程序！";
//            }
//            if (!File.Exists(sourceFilePath))
//            {
//                return "找不到源文件！";
//            }

//            //文件扩展名
//            string fileExt = GetDiscription(videotype);

//            //创建的文件夹
//            var saveDir = DateTime.Now.ToString("yyyyMMddHHmmss");
//            var videoname = filename.Split('.')[0];
//            string chDir = "/Resource/Video/" + saveDir + "/" + videoname + fileExt;

//            var d = new DirectoryInfo(TargetFilepath + "\\Resource\\Video\\" + saveDir);
//            if (!d.Exists) d.Create();
//            var destPath = d.FullName + "\\" + videoname + fileExt;
//            var fileInfos = new FileInfo(destPath);
//            System.Diagnostics.ProcessStartInfo FilestartInfo = new System.Diagnostics.ProcessStartInfo(ffmpeg);
//            FilestartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
//            /*ffmpeg参数说明
//             * -i 1.avi   输入文件
//             * -ab/-ac <比特率> 设定声音比特率，前面-ac设为立体声时要以一半比特率来设置，比如192kbps的就设成96，转换 
//                均默认比特率都较小，要听到较高品质声音的话建议设到160kbps（80）以上
//             * -ar <采样率> 设定声音采样率，PSP只认24000
//             * -b <比特率> 指定压缩比特率，似乎ffmpeg是自动VBR的，指定了就大概是平均比特率，比如768，1500这样的   --加了以后转换不正常
//             * -r 29.97 桢速率（可以改，确认非标准桢率会导致音画不同步，所以只能设定为15或者29.97）
//             * s 320x240 指定分辨率
//             * 最后的路径为目标文件
//             */
//            FilestartInfo.Arguments = " -i \"" + sourceFilePath + "\"  -hls_time 30 -c:v libx264 -hls_list_size 0 -c:a aac  -strict -2 -f hls " + destPath;
//            //FilestartInfo.Arguments = " -i " + sourceFilePath + " -ab 80 -ar 22050 -r 29.97 -s " + videosize + " " + destPath;
//            //FilestartInfo.Arguments = "-y -i " + sourceFile + " -s 320x240 -vcodec h264 -qscale 4  -ar 24000 -f psp -muxvb 768 " + destFile;
//            try
//            {
//                //转换
//                System.Diagnostics.Process.Start(FilestartInfo);
//                //destVideo = destPath.Split('.')[0] + "/" + filename;
//            }
//            catch
//            {
//                return "格式转换失败！";
//            }
//            //}
//            //格式不需要转换则直接复制文件到目录
//            //else
//            //{
//            //    File.Copy(sourceFile, destFile,true);
//            //    destVideo = destPath + filename;
//            //}
//            //提取视频长度
//            if (getDuration)
//            {
//                VideoLength = GetVideoDuration(sourceFilePath);
//            }
//            //提取图片
//            if (createImage)
//            {
//                //定义进程
//                System.Diagnostics.ProcessStartInfo ImgstartInfo = new System.Diagnostics.ProcessStartInfo(ffmpeg);

//                //截大图
//                string imgpath = d.FullName + "\\"  + videoname + ".jpg";
//                ConvertImage(sourceFilePath, imgpath, imgsize, ImgstartInfo);

//                //截小图
//                imgpath = d.FullName + "\\"  + videoname + "_thumb.jpg";
//                DestImage = ConvertImage(sourceFilePath, imgpath, "80*80", ImgstartInfo);

//            }
//            return chDir;
//        }
//        /// <summary>
//        /// 创建图标
//        /// </summary>
//        /// <param name="sourceFile"></param>
//        /// <param name="imgpath"></param>
//        /// <param name="imgsize"></param>
//        /// <param name="ImgstartInfo"></param>
//        /// <returns></returns>
//        public static string ConvertImage(string sourceFile, string imgpath, string imgsize, System.Diagnostics.ProcessStartInfo ImgstartInfo)
//        {
//            ImgstartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
//            /*参数设置
//             * -y（覆盖输出文件，即如果生成的文件（flv_img）已经存在的话，不经提示就覆盖掉了）
//             * -i 1.avi 输入文件
//             * -f image2 指定输出格式
//             * -ss 8 后跟的单位为秒，从指定时间点开始转换任务
//             * -vframes
//             * -s 指定分辨率
//             */
//            //duration: 00:00:00.00
//            string[] time = VideoLength.Split(':');
//            int seconds = int.Parse(time[0]) * 60 * 60 + int.Parse(time[1]) * 60 + int.Parse(time[2]);
//            int ss = seconds > 5 ? 5 : seconds - 1;
//            ImgstartInfo.Arguments = " -i " + sourceFile + " -y -f image2 -ss " + ss.ToString() + " -vframes 1 -s " + imgsize + " " + imgpath;
//            try
//            {
//                System.Diagnostics.Process.Start(ImgstartInfo);
//                return imgpath;
//            }
//            catch
//            {
//                return "";
//            }
//        }

//        /// <summary>
//        /// 获取视频时长
//        /// </summary>
//        /// <param name="sourceFile"></param>
//        /// <returns></returns>
//        public static string GetVideoDuration(string sourceFile)
//        {
//            sourceFile = TargetFilepath + "\\" + sourceFile;
//            using (System.Diagnostics.Process ffmpeg = new System.Diagnostics.Process())
//            {
//                String duration;  // soon will hold our video's duration in the form "HH:MM:SS.UU"
//                String result;  // temp variable holding a string representation of our video's duration
//                StreamReader errorreader;  // StringWriter to hold output from ffmpeg

//                // we want to execute the process without opening a shell
//                ffmpeg.StartInfo.UseShellExecute = false;
//                //ffmpeg.StartInfo.ErrorDialog = false;
//                ffmpeg.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
//                // redirect StandardError so we can parse it
//                // for some reason the output comes through over StandardError
//                ffmpeg.StartInfo.RedirectStandardError = true;

//                // set the file name of our process, including the full path
//                // (as well as quotes, as if you were calling it from the command-line)
//                ffmpeg.StartInfo.FileName = fileDir;

//                // set the command-line arguments of our process, including full paths of any files
//                // (as well as quotes, as if you were passing these arguments on the command-line)
//                ffmpeg.StartInfo.Arguments = "-i " + sourceFile;

//                // start the process
//                ffmpeg.Start();

//                // now that the process is started, we can redirect output to the StreamReader we defined
//                errorreader = ffmpeg.StandardError;

//                // wait until ffmpeg comes back
//                ffmpeg.WaitForExit();

//                // read the output from ffmpeg, which for some reason is found in Process.StandardError
//                result = errorreader.ReadToEnd();

//                // a little convoluded, this string manipulation...
//                // working from the inside out, it:
//                // takes a substring of result, starting from the end of the "Duration: " label contained within,
//                // (execute "ffmpeg.exe -i somevideofile" on the command-line to verify for yourself that it is there)
//                // and going the full length of the timestamp

//                duration = result.Substring(result.IndexOf("Duration: ") + ("Duration: ").Length, ("00:00:00").Length);
//                return duration;
//            }
//        }

//        #endregion
//    }
//}
