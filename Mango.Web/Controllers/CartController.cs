using Mango.Web.Models;
using Mango.Web.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mango.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICartService _cartService;

        public CartController(IProductService productService,ICartService cartService)
        {
            _productService = productService;
            _cartService = cartService;
        }
        public async Task<IActionResult> CartIndex()
        {
            return View(await GetCartOfUser());
        }
        public async Task<IActionResult> RemoveCartItem(int cartDetailsId)
        {
            var access_token = await HttpContext.GetTokenAsync("access_token");
            var response = await _cartService.RemoveFromCartAsync<ResponseDto>(cartDetailsId, access_token);
            IActionResult actionResult = View();
            if (response != null && response.IsSuccess)
            {
                bool isRemoved = Convert.ToBoolean(response.Result);

                actionResult = isRemoved ? RedirectToAction(nameof(CartIndex)) : View();
            }
            return actionResult;
        }

        private async Task<CartDto> GetCartOfUser() 
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(u => u.Type == "sub")?.Value;
            var access_token =await HttpContext.GetTokenAsync("access_token");
            var response = await _cartService.GetCartByUserIdAsnyc<ResponseDto>(userId, access_token);
            CartDto cart = new CartDto();
            if (response != null && response.IsSuccess) 
            {
                cart = JsonConvert.DeserializeObject<CartDto>(response.Result.ToString());
                if (cart.CartHeader != null)
                {
                    foreach (var cartDetail in cart.CartDetails)
                    {
                        cart.CartHeader.OrderTotal += cartDetail.Count * cartDetail.Product.Price;
                    }
                }
            }
            return cart;        
        }
    }
}
