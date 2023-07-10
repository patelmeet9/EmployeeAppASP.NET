using Lane4Task.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Lane4Task.Data
{
    public class UserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetUserByEmail(string email)
        {
            return await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
        }

        public void InsertUser(User user)
        {
            var firstNameParam = new SqlParameter("@FirstName", user.FirstName);
            var lastNameParam = new SqlParameter("@LastName", user.LastName);
            var emailParam = new SqlParameter("@Email", user.Email);
            var mobileNumberParam = new SqlParameter("@MobileNumber", user.MobileNumber);
            var passwordParam = new SqlParameter("@Password", user.Password);
            var ageParam = new SqlParameter("@Age", user.Age);
            var genderParam = new SqlParameter("@Gender", user.Gender);

            _context.Database.ExecuteSqlRaw("EXEC InsertUser @FirstName, @LastName, @Email, @MobileNumber, @Password, @Age, @Gender",
                firstNameParam, lastNameParam, emailParam, mobileNumberParam, passwordParam, ageParam, genderParam);
        }

        public bool CheckIfFriend(string loggedInEmail, string friendEmail)
        {
            var user = _context.Users
                .Include(u => u.Friends)
                .FirstOrDefault(u => u.Email == loggedInEmail);

            if (user != null)
            {
                return user.Friends.Any(f => f.FriendUser.Email == friendEmail);
            }

            return false;
        }
    }
}
