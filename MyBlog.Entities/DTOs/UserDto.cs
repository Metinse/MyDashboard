namespace MyBlog.DTOs
{
    public class UserDto
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        // Şifreyi DTO'ya eklemiyoruz, güvenlik açısından hassas veriyi saklamıyoruz.
    }
}
