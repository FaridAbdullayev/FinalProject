﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Dtos.Review
{
    public class ReviewGetDto
    {

        public int Id { get; set; }
        public string? UserName { get; set; }
        public string RoomName { get; set; }
        public string Text { get; set; }
        public string Status { get; set; }
        public byte Rate { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
