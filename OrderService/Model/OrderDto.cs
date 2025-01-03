using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderService.Model
{
    public class OrderDto
    {
        public int? OrderId { get; set; }
        public int? ProductId { get; set; }
        public bool? VisableFlag { get; set; }
    }
}