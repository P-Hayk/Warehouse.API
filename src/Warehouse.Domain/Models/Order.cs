using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warehouse.Domain.Models
{
    public class Order
    {
        public int ClientId { get; set; }
        public int ProductId { get; set; }
        public OrderState State { get; set; }
        public DateTime DateTime { get; set; }
        public Product Product { get; set; }
        public Client Client { get; set; }
    }

    public enum OrderState
    {
        Approved = 1,
        Rejected = 2,
        UnderReview = 3,
        Pending = 4,
    }
}
