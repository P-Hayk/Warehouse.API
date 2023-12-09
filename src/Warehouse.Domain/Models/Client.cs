using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warehouse.Domain.Models
{
    public class Client
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public List<Order> Orders { get; set; }
    }
}
