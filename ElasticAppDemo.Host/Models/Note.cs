using System.Runtime.InteropServices;

namespace ElasticAppDemo.Host.Models
{
    public class Note : ElasticModelBase
    {
        /**
    * 笔记ID
    */
        public long noteId { get; set; }

        /**
         * 封面
         */
        public String cover { get; set; }

        /**
         * 标题
         */
        public String title { get; set; }

        /**
         * 标题：关键词高亮
         */
        public String highlightTitle { get; set; }

        /**
         * 发布者头像
         */
        public String avatar { get; set; }

        /**
         * 发布者昵称
         */
        public String nickname { get; set; }

        /**
         * 最后一次编辑时间
         */
        public String updateTime { get; set; }

        /**
         * 被点赞总数
         */
        public String likeTotal { get; set; }
        /**
         * 被评论数
         */
        public String commentTotal { get; set; }

        /**
         * 被收藏数
         */
        public String collectTotal { get; set; }
    }
}
