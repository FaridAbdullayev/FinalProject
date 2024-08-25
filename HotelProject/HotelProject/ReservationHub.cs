using Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace HotelProject
{
    public class ReservationHub:Hub
    {
        public async Task SendReservationAlert(string message)
        {
            await Clients.All.SendAsync("ReceiveReservationAlert", message);
        }
    }
}
