using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warehouse.Domain.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Stock { get; set; }
        public ProductState State
        {
            get
            {
                if (Stock == 0)
                {
                    return ProductState.OutOfStock;
                }
                else if (Stock < Category?.StockThreshold)
                {
                    return ProductState.LowStock;
                }

                return ProductState.Available;
            }
        }
        public Category Category { get; set; }
        public int CategoryId { get; set; }
    }

    public enum ProductState
    {
        Available,
        LowStock,
        OutOfStock
    }
}