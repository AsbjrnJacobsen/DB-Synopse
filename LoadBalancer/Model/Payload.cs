using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LoadBalancer.Model
{
    public class Payload
    {
        [Required]
        public OrderDto OrderDto{ get; set; }
        [Required]
        public int Quantity { get; set; }
    }
}