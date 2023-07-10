using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lane4Task.Models
{
    public class Friend
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        [ForeignKey("FriendUser")]
        public int FriendId { get; set; }

        

        // Navigation properties

        public virtual User User { get; set; }

        [InverseProperty("Friends")]
        public virtual User FriendUser { get; set; }
    }
}
