﻿using FluentValidation;
using Service.Dtos.Room;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Dtos.Users
{
    public class RoomPreReservationInfoDto
    {
        public string RoomName { get; set; }
        public string BrnachName { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int Nights { get; set; }
        public int ChildrenCount { get; set; }
        public int AdultsCount { get; set; }
        public double TotalPrice { get; set; }
        public string ImageUrl {  get; set; }
    }
}
