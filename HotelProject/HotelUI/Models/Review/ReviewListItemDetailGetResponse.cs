namespace HotelUI.Models.Review
{
    public class ReviewListItemDetailGetResponse
    {

        public string? FullName { get; set; }
        public int RoomId { get; set; }
        public string Text { get; set; }
        public string Status { get; set; }
        public byte Rate { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
