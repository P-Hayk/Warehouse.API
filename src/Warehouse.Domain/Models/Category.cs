﻿namespace Warehouse.Domain.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int StockThreshold { get; set; }
    }
}
