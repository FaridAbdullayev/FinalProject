﻿using HotelUI.Models.Enums;

namespace HotelUI.Models.Room
{
    public class RoomGetResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public double Area { get; set; }
        public int MaxChildrenCount { get; set; }
        public int MaxAdultsCount { get; set; }
        public string BedType { get; set; }
        public int BranchId { get; set; }
        public List<FileResponse> Images { get; set; }
        public List<int> ServiceIds { get; set; }
    }
}