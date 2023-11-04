using System.ComponentModel;

namespace Laoyoutiao.Extends
{
    //文件类型
    public enum VideoType
    {
        [Description(".avi")]
        AVI,
        [Description(".mov")]
        MOV,
        [Description(".mpg")]
        MPG,
        [Description(".mp4")]
        MP4,
        [Description(".flv")]
        FLV,
        [Description(".m3u8")]
        M3U8
    }
}
