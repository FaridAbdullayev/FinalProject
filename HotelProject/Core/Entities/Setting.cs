using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Setting
    {
        [Key]
        public string Key { get; set; }
        [MaxLength(500)]
        public string Value { get; set; }
    }
}
