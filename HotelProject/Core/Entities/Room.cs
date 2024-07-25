using Core.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Room:AuditEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image {  get; set; }
        public double Price { get; set; }
        public double Area { get; set; }
        public int MaxChildrenCount {  get; set; }
        public int MaxAdultsCount { get; set; }
        public BedType BedType { get; set; }
        public int BranchId {  get; set; }
        public Branch Branch { get; set; }
        public List<RoomService> RoomServices { get; set; }
    }
}
