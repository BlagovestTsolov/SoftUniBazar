using System.ComponentModel.DataAnnotations;
using static SoftUniBazar.Data.Constants.DataConstants;

namespace SoftUniBazar.Data.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(CategoryNameMaxLength)]
        public string Name { get; set; } = null!;

        public IList<Ad> Ads { get; set; } 
            = new List<Ad>();
    }
}