namespace Laoyoutiao.Models.Dto.Menu
{
    public class MenuEdit: BaseDto
    {       
        public string? Name { get; set; }
        public string? Index { get; set; }
        public string? FilePath { get; set; }
        public long ParentId { get; set; }
        public int Order { get; set; }
        public bool IsEnable { get; set; }
        public string? Description { get; set; }
    }
}
