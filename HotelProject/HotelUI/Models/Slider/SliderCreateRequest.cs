namespace HotelUI.Models.Slider
{
    public class SliderCreateRequest
    {
        public IFormFile File { get; set; }
        public string Description { get; set; }
        public int Order { get; set; }
    }
}
