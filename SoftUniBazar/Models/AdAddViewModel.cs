using System.ComponentModel.DataAnnotations;
using static SoftUniBazar.Data.Constants.DataConstants;
using static SoftUniBazar.Data.ErrorMessages.ValidationErrorMessages;

namespace SoftUniBazar.Models
{
    public class AdAddViewModel
    {
        [Required(ErrorMessage = RequiredErrorMessage)]
        [StringLength(AdNameMaxLength,
            MinimumLength = AdNameMinLength,
            ErrorMessage = LengthErrorMessage)]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = RequiredErrorMessage)]
        [StringLength(AdDescriptionMaxLength,
            MinimumLength = AdDescriptionMinLength,
            ErrorMessage = LengthErrorMessage)]
        public string Description { get; set; } = null!;

        [Required(ErrorMessage = RequiredErrorMessage)]
        public decimal Price { get; set; }

        [Required(ErrorMessage = RequiredErrorMessage)]
        public string ImageUrl { get; set; } = null!;

        [Required(ErrorMessage = RequiredErrorMessage)]
        public int CategoryId { get; set; }

        public IList<CategoryViewModel> Categories { get; set; } = new List<CategoryViewModel>();
    }
}
