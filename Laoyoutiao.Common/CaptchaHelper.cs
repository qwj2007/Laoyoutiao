using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.IO;


namespace Laoyoutiao.Common
{
    public static class CaptchaHelper
    {
        private static readonly Random _random = new Random();
        /// <summary>
        /// 生成验证码字符串
        /// </summary>
        public static string GenerateCode(int length = 4)
        {
            const string chars = "23456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghigkmnpqrstuvwxyz";
            char[] result = new char[length];

            for (int i = 0; i < length; i++)
            {
                result[i] = chars[_random.Next(chars.Length)];
            }

            return new string(result);
        }
        /// <summary>
        /// 生成图片验证码
        /// </summary>
        /// <param name="code"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static byte[] GenerateCaptchaImage(string code, int width = 130, int height = 48)
        {
            using var image = new Image<Rgba32>(width, height);
            image.Mutate(x => x.BackgroundColor(Color.White));

            var font = SystemFonts.CreateFont("Arial", 28, FontStyle.Bold);

            // 干扰线
            for (int i = 0; i < 10; i++)
            {
                var color = Color.FromRgb(
                    (byte)_random.Next(100, 180),
                    (byte)_random.Next(100, 180),
                    (byte)_random.Next(100, 180));

                image.Mutate(x => x.DrawLine(
                    color,
                    1,
                    new PointF(_random.Next(width), _random.Next(height)),
                    new PointF(_random.Next(width), _random.Next(height))
                ));
            }

            // 噪点
            for (int i = 0; i < 120; i++)
            {
                int x = _random.Next(width);
                int y = _random.Next(height);
                image[x, y] = Color.FromRgb(
                    (byte)_random.Next(0, 255),
                    (byte)_random.Next(0, 255),
                    (byte)_random.Next(0, 255));
            }

            // ====================== 最终修复：居中绘制文字 ======================
            var location = new PointF(width / 4, height / 4);

            image.Mutate(x => x.DrawText(
                code,
                font,
                Color.Black,
                location
            ));

            using var ms = new MemoryStream();
            image.SaveAsPng(ms);
            return ms.ToArray();
        }
    }
}
