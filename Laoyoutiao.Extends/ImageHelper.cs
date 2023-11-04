using System.Drawing;
using System.Drawing.Imaging;

namespace Laoyoutiao.Extends
{
    /// <summary>
    /// 图片帮助文件
    /// </summary>
    public class ImageHelper
    {
        //文件保存路径
        public static string FilepathDir
        {
            get
            {
                return AppDomain.CurrentDomain.BaseDirectory;
            }
        }
        /// <summary>
        /// 给图片添加文字
        /// </summary>
        /// <param name="fileUrl"></param>
        /// <returns></returns>
        public static string SetFontToImage(string fileSourceUrl, string texts)
        {
            string fullPath = FilepathDir + "\\" + fileSourceUrl;
            string fileName = Path.GetFileName(fullPath);
            //读取文件名称           
            using (var stream = new MemoryStream())
            {
                FileStream file = new FileStream(fullPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                byte[] bytes = new byte[file.Length];
                file.Read(bytes, 0, (int)file.Length);
                stream.Write(bytes, 0, (int)file.Length);
                file.Close();
                var watermarkedStream = new MemoryStream();
                using (var img = Image.FromStream(stream))
                {
                    using (var graphic = Graphics.FromImage(img))
                    {
                        var font = new Font(FontFamily.GenericSansSerif, 20, FontStyle.Bold, GraphicsUnit.Pixel);
                        var color = Color.Red;// Color.FromArgb(0, 255, 255, 255);
                        var brush = new SolidBrush(color);
                        var point = new Point(img.Width / 2 - 200, img.Height / 2 - 100);
                        graphic.DrawString(texts, font, brush, point);
                        img.Save(watermarkedStream, ImageFormat.Png);
                    }
                    var d = new DirectoryInfo(FilepathDir + "/Resource/ZS/");
                    if (!d.Exists) d.Create();
                    img.Save(FilepathDir + "/Resource/ZS/" + fileName);
                }
            }
            return "/Resource/ZS/" + fileName;

        }
    }
}

