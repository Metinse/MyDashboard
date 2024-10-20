namespace MyBlog.Entities
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        public DateTime AddDate { get; set; } = DateTime.Now;
        public DateTime? UpdateDate { get; set; }
        public bool Status { get; set; } = true;
    }
}
