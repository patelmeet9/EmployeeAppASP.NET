namespace Lane4Task.Models
{
    public class AllData : User
    {
        public List<User> Users { get; set; }
        public List<Friend> Friends { get; set; }
    }
}
