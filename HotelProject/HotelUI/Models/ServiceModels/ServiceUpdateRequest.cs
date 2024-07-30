using System.ComponentModel.DataAnnotations;

namespace HotelUI.Models.ServiceModels
{
    public class ServiceUpdateRequest
    {
        [MaxLength(50)]
        [MinLength(3)]
        [Required]
        public string Name { get; set; }
    }
}
