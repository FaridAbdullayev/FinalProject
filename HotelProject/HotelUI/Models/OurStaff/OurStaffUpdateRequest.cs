﻿namespace HotelUI.Models.OurStaff
{
    public class OurStaffUpdateRequest
    {
        public string Name { get; set; }
        public string Position { get; set; }
        public string Description { get; set; }
        public IFormFile? File { get; set; }
    }
}
