using Dapper;
using MyBlog.Entities;
using System.Data;
using System.Linq;

namespace MyBlog.DataAccess.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbConnection _dbConnection;

        public UserRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public User GetUserByUsernameAndPassword(string username, string password)
        {
            string query = "SELECT * FROM [User] WHERE UserName = @Username AND Password = @Password AND Status = 1";
            return _dbConnection.Query<User>(query, new { Username = username, Password = password }).FirstOrDefault();
        }
    }
}
