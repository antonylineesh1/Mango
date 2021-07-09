﻿using Mango.Services.ShoppingCartAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shoppingcart.API.Models
{
    public class Cart
    {
        public CartHeader CartHeader { get; set; }
        public IEnumerable<CartDetails> CartDetails { get; set; }
    }
}