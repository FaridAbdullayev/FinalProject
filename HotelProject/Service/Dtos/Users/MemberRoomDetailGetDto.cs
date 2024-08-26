﻿using Service.Dtos.Room;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Dtos.Users
{
    public class MemberRoomDetailGetDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public double Area { get; set; }
        public int MaxChildrenCount { get; set; }
        public int MaxAdultsCount { get; set; }
        public int BranchId { get; set; }
        public int BedTypeId { get; set; }
        public List<int> ServiceIds { get; set; } = new List<int>();
        public List<MemberRoomImageGetDto> Images { get; set; } = new List<MemberRoomImageGetDto>();
    }
}