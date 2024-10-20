namespace MyBlog.Entities
{
    public class User : BaseEntity
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; } // Şifre sadece entity'de olacak, DTO'da olmayacak
        public string Role { get; set; }
    }
}
