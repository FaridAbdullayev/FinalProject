namespace HotelUI.Models.Slider
{
    public class SliderUpdateRequest
    {
        public IFormFile? File { get; set; }
        public string Description { get; set; }
        public int Order { get; set; }
    }
}
