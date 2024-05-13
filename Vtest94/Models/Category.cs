using System.ComponentModel.DataAnnotations;

namespace Vtest94.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        // Navigation property to support EF Core relationships
        public virtual ICollection<Video> Videos { get; set; }
    }
}
