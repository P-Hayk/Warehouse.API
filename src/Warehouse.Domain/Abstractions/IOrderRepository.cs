﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Warehouse.Domain.Models;

namespace Warehouse.Domain.Abstractions;
public interface IOrderRepository
{
    Task CreateAsync(Order order);
}
