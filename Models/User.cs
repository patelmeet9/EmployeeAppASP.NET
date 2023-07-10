using System.ComponentModel.DataAnnotations;

namespace Lane4Task.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int MobileNumber { get; set; }
        public string Password { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public DateTime RegisterDate { get; set; } = DateTime.Now;
        public virtual ICollection<Friend> Friends { get; set; } = new List<Friend>();
    }
}
