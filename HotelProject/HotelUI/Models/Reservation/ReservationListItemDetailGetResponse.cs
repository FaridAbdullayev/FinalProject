namespace HotelUI.Models.Reservation
{
    public class ReservationListItemDetailGetResponse
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? UserName { get; set; }
        public string RoomName { get; set; }
        public string Status { get; set; }
    }
}
