using Mango.Web.Models;
using Mango.Web.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
//using System.Text.Json;
using System.Threading.Tasks;

namespace Mango.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;
        private readonly ICartService _cartService;

        public HomeController(ILogger<HomeController> logger, IProductService productService,
            ICartService cartService)
        {
            _logger = logger;
            _productService = productService;
            _cartService = cartService;
        }

        public async Task<IActionResult> Index()
        {
            List<ProductDto> list = new();
            var response = await _productService.GetAllProductsAsync<ResponseDto>(string.Empty);
            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(response.Result));
            }
            return View(list);
        }

        [Authorize]
        public async Task<IActionResult> Details(int productId)
        {
            ProductDto model = new();
            var response = await _productService.GetProductByIdAsync<ResponseDto>(productId, "");
            if (response != null && response.IsSuccess)
            {
                model = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
            }
            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ActionName("Details")]
        public async Task<IActionResult> DetailsPost(ProductDto product)
        {
           var test= User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value;

            CartDto cart = new CartDto
            {
                CartHeader = new CartHeaderDto
                {
                    UserId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value
                }
            };

            CartDetailsDto cartDetails = new CartDetailsDto
            {
                Count = product.Count,
                ProductId = product.ProductId
            };

            var reponse =await _productService.GetProductByIdAsync<ResponseDto>(product.ProductId,string.Empty); 
            if(reponse!= null && reponse.IsSuccess)
            {
                cartDetails.Product = JsonConvert.DeserializeObject<ProductDto>(reponse.Result.ToString());
            }
            var cartDetailslist = new List<CartDetailsDto>();
            cartDetailslist.Add(cartDetails);
            cart.CartDetails = cartDetailslist;
            string accessToken = await HttpContext.GetTokenAsync("access_token");

            var cartRespo= await _cartService.AddToCartAsync<ResponseDto>(cart, accessToken);
            if (cartRespo != null && cartRespo.IsSuccess)
            {
                return RedirectToAction(nameof(Index));
            }

            //return View('Details', product);
            return View(product);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [Authorize]
        public IActionResult Login()
        {
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Logout()
        {
            return SignOut("Cookies", "oidc");
        }
    }
}
