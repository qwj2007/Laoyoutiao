namespace ElasticAppDemo.Host.Models
{
    public class NoteAgg
    {
        public int countVal { get; set; }

        /**
         * 被点赞总数
         */
        public int likeTotal { get; set; }
        /**
         * 被评论数
         */
        public int commentTotal { get; set; }

        /**
         * 被收藏数
         */
        public int collectTotal { get; set; }
    }
}
