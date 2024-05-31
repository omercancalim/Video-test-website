namespace Vtest94.Models
{
    public class UserLikes
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int VideoId { get; set; }
        public bool IsLike { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
