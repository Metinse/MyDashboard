using MyBlog.Entities;

namespace MyBlog.DataAccess.Repositories
{
    public interface IUserRepository
    {
        User GetUserByUsernameAndPassword(string username, string password);
    }
}
