using System.ComponentModel.DataAnnotations;

namespace HotelUI.Models.Branch
{
    public class BranchCreateRequest
    {
        [MaxLength(50)]
        [MinLength(3)]
        [Required]
        public string Name { get; set; }
    }
}
