using System.Text.Json.Serialization;

namespace HotelUI.Models.Reservation
{
    public class MonthlyIncomeResponse
    {
        [JsonPropertyName("months")]
        public List<string> Months { get; set; }

        [JsonPropertyName("appointments")]
        public List<int> Reservation { get; set; }
    }
}
