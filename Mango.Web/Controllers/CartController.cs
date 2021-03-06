using Mango.Web.Models;
using Mango.Web.Models.Dto;
using Mango.Web.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
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
        public ICouponService _couponService { get; }

        public CartController(IProductService productService,ICartService cartService,ICouponService couponService)
        {
            _productService = productService;
            _cartService = cartService;
            _couponService = couponService;
        }
        [Authorize]
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

        [HttpPost]
        public async Task<IActionResult> Checkout(CartDto cartDto)
        {
            try
            {
                var accessToken = await HttpContext.GetTokenAsync("access_token");
                var response = await _cartService.Checkout<ResponseDto>(cartDto.CartHeader, accessToken);
                return RedirectToAction(nameof(Confirmation));
            }
            catch (Exception e)
            {
                return View(cartDto);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            return View(await GetCartOfUser());
        }

        [HttpGet]
        public IActionResult Confirmation()
        {
            return View();
        }

        [HttpPost]
        [ActionName("ApplyCoupon")]
        public async Task<IActionResult> ApplyCoupon(CartDto cartDto)
        {
            var userId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value;
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _cartService.ApplyCoupon<ResponseDto>(cartDto, accessToken);

            if (response != null && response.IsSuccess)
            {
                return RedirectToAction(nameof(CartIndex));
            }
            return View();
        }

        [HttpPost]
        [ActionName("RemoveCoupon")]
        public async Task<IActionResult> RemoveCoupon(CartDto cartDto)
        {
            var userId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value;
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var response = await _cartService.RemoveCoupon<ResponseDto>(cartDto.CartHeader.UserId, accessToken);

            if (response != null && response.IsSuccess)
            {
                return RedirectToAction(nameof(CartIndex));
            }
            return View();
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
                    if (!string.IsNullOrEmpty(cart.CartHeader.CouponCode))
                    {
                        var couponResp = await _couponService.GetCoupon<ResponseDto>(cart.CartHeader.CouponCode, string.Empty);
                        if(couponResp !=null && couponResp.IsSuccess)
                        {
                            var couponDto = JsonConvert.DeserializeObject<CouponDto>(couponResp.Result.ToString());
                            cart.CartHeader.DiscountTotal = couponDto.DiscountAmount;
                        }

                    }
                    foreach (var cartDetail in cart.CartDetails)
                    {
                        cart.CartHeader.OrderTotal += cartDetail.Count * cartDetail.Product.Price;
                    }
                    cart.CartHeader.OrderTotal -= cart.CartHeader.DiscountTotal;
                }
            }
            return cart;        
        }
    }
}
