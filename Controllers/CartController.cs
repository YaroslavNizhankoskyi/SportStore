using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SportStore.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using SportStore.Models.Repositories;
using SportStore.Infrastructure;
using SportStore.ViewModels;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace SportStore.Controllers
{
    [Authorize]
    public class CartController : Controller
    {

        private IOutfitRepository OutfitRepository;
        private Cart cart;
        private UserManager<Customer> userManager;
        private readonly IHttpContextAccessor httpContextAccessor;
        private SignInManager<Customer> signInManager;
        private IOrderRepository OrderRepository;
        private IOrderedOutfitRepository orderedOutfitRepository;

        public CartController(IOutfitRepository repo, Cart cartService, OrderEditor orderService,
            UserManager<Customer> userManager, IHttpContextAccessor httpContextAccessor, IOrderRepository orderRepository, IOrderedOutfitRepository orderedOutfitRepository)
        {
            OutfitRepository = repo;
            cart = cartService;
            this.userManager = userManager;
            this.httpContextAccessor = httpContextAccessor;
            signInManager = signInManager;
            OrderRepository = orderRepository;
            this.orderedOutfitRepository = orderedOutfitRepository;
        }


        public ViewResult CartIndex(string returnUrl)
        {
            return View(new CartIndexViewModel
            {
                Cart = cart,
                ReturnUrl = returnUrl
            });
        }

        public RedirectToActionResult AddToCart(int OutfitId, string returnUrl)
        {
            Outfit Outfit = OutfitRepository.Get(OutfitId);

            if (Outfit != null)
            {
                cart.AddLine(Outfit, 1);
                SaveCart(cart);
            }
            ViewBag.Price = cart.Sum();
            ViewBag.OutfitCount = cart.Lines.Count;
            return RedirectToAction("CartIndex", new { returnUrl });
        }

        public IActionResult Try(int quantity)
        {
            return View(quantity);
        }

        

        private Cart GetCart()
        {
            Cart cart = HttpContext.Session.GetJson<Cart>("Cart") ?? new Cart();
            return cart;
        }

        private void SaveCart(Cart cart)
        {
            HttpContext.Session.SetJson("Cart", cart);
        }

        public async Task<IActionResult> Order(string userName)
        {
            Customer customer = await userManager.FindByNameAsync(userName);
            Cart cart = GetCart();

            Order order = OrderRepository.AddOrder(cart, customer);
            orderedOutfitRepository.AddOrderedOutfits(cart.Lines, order.Id);
            cart.Clear();
            return RedirectToAction("UserOrders", "Order");

        }

        public async Task<IActionResult> UserOrders()
        {
            var userId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = await userManager.FindByIdAsync(userId);
            int sum = 0;

            IEnumerable<Order> orders = OrderRepository.GetOrders(user.Id);

            UserOrdersVM userOrders = new UserOrdersVM();


            foreach (Order o in orders)
            {
                OrderVM orderVm = new OrderVM();
                
                foreach (OrderedOutfit ob in orderedOutfitRepository.GetOrderedOutfits(o.Id))
                {
                    CartLine ct = new CartLine
                    {
                        Outfit = OutfitRepository.Get(ob.OutfitId),
                        Quantity = ob.Quantity
                    };
                    orderVm.Lines.Add(ct);
                }
                orderVm.OrderId = o.Id;

                userOrders.Orders.Add(orderVm);

                sum += o.Price;
                
            }
            userOrders.Price = sum;
            return View(userOrders);
        }


        //CHANGE LINE
        [HttpPost]
        public IActionResult ChangeLine(int OutfitId, int quantity, string ReturnUrl)
        {
            cart.ChangeLine(OutfitId, quantity);
            TempData["message"] = "Changed";
            return RedirectToAction("CartIndex", new { ReturnUrl });
        }

        public IActionResult RemoveFromCart(int OutfitId, string ReturnUrl)
        {
            cart.RemoveLine(OutfitId);
            TempData["message"] = "Removed";
            return RedirectToAction("CartIndex", new { ReturnUrl });
        }





    }
}
