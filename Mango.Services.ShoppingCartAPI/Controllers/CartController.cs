using Mango.Services.ShoppingCartAPI.Models.Dto;
using Mango.Services.ShoppingCartAPI.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mango.Services.ShoppingCartAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartRepository _cartRepository;
        private ResponseDto _reponse;

        public CartController(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
            _reponse = new ResponseDto();
        }

        [HttpGet("GetCart/{userId}")]
        public async Task<object> GetCart(string userId)
        {
            try
            {
                _reponse.Result = await _cartRepository.GetCartByUserId(userId);
            }
            catch (Exception ex)
            {
                _reponse.IsSuccess = false;
                _reponse.ErrorMessages = new List<string> { ex.Message };
            }
            return _reponse;
        }

        [HttpPost("UpdateCart")]
        public async Task<object> UpdateCart(CartDto cartDto)
        {
            try
            {
                _reponse.Result = await _cartRepository.CreateUpdateCart(cartDto);
            }
            catch (Exception ex)
            {
                _reponse.IsSuccess = false;
                _reponse.ErrorMessages = new List<string> { ex.Message };
            }
            return _reponse;
        }

        [Authorize]
        [HttpPost("AddCart")]
        public async Task<object> AddCart(CartDto cartDto)
        {
            try
            {
                _reponse.Result = await _cartRepository.CreateUpdateCart(cartDto);
            }
            catch (Exception ex)
            {
                _reponse.IsSuccess = false;
                _reponse.ErrorMessages = new List<string> { ex.Message };
            }
            return _reponse;
        }

        [HttpPost("RemoveFromCart")]
        public async Task<object> RemoveFromCart(int cartId)
        {
            try
            {
                _reponse.Result = await _cartRepository.RemoveFromCart(cartId);
            }
            catch (Exception ex)
            {
                _reponse.IsSuccess = false;
                _reponse.ErrorMessages = new List<string> { ex.Message };
            }
            return _reponse;
        }

        [HttpPost("ApplyCoupon")]
        public async Task<object> ApplyCoupon(CartDto cart)
        {
            try
            {
                _reponse.Result = await _cartRepository.ApplyCoupon(cart.CartHeader.UserId,cart.CartHeader.CouponCode);
            }
            catch (Exception ex)
            {
                _reponse.IsSuccess = false;
                _reponse.ErrorMessages = new List<string> { ex.Message };
            }
            return _reponse;
        }

        [HttpPost("RemoveCoupon")]
        public async Task<object> RemoveCoupon(string userId)
        {
            try
            {
                _reponse.Result = await _cartRepository.RemoveCoupon(userId);
            }
            catch (Exception ex)
            {
                _reponse.IsSuccess = false;
                _reponse.ErrorMessages = new List<string> { ex.Message };
            }
            return _reponse;
        }
    }
}
