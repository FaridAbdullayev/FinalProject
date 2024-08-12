namespace HotelUI.Models.Review
{
    public class ReviewListItemDetailGetResponse
    {
        public int Id { get; set; }
        public string RoomName { get; set; }
        public string? UserName { get; set; }
        public string Status { get; set; }
        public byte Rate { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
