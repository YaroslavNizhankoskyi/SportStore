using SportStore.Infrastructure;
using SportStore.Models;
using SportStore.Models.Repositories;
using SportStore.TagHelpers;
using SportStore.ViewModels;
using SportStore.ViewModels.Order;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Syncfusion.HtmlConverter;
using Syncfusion.Pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SportStore.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private  IOrderedOutfitRepository orderedOutfitRepository;
        private  IOrderRepository orderRepository;
        [JsonIgnore]
        private OrderEditor order;
        private  IHttpContextAccessor httpContextAccessor;
        private  IOutfitRepository OutfitRepository;
        private  UserManager<Customer> userManager;
        private readonly IHostingEnvironment hostingEnvironment;


        public OrderController(IOrderedOutfitRepository orderedOutfitRepository, IOrderRepository orderRepository, OrderEditor order,
                 IHttpContextAccessor httpContextAccessor, IOutfitRepository OutfitRepository
            , UserManager<Customer> userManager, IHostingEnvironment hostingEnvironment)
        {
            this.orderedOutfitRepository = orderedOutfitRepository;
            this.orderRepository = orderRepository;
            this.order = order;
            this.httpContextAccessor = httpContextAccessor;
            this.OutfitRepository = OutfitRepository;
            this.userManager = userManager;
            this.hostingEnvironment = hostingEnvironment;
        }

        private OrderEditor GetOrderEditor()
        {
            OrderEditor order = HttpContext.Session.GetJson<OrderEditor>("Order") ?? new OrderEditor();
            return order;
        }
        
        private void SaveOrderEditor(OrderEditor order)
        {
            HttpContext.Session.SetJson("Order", order);
        }

        [HttpGet]
        public async Task<IActionResult> UserOrders(string sort = "DSC")
        {
            order.Clear();
            var userId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = await userManager.FindByIdAsync(userId);
            int sum = 0;
            int number = 0;

            IEnumerable<Order> orders = orderRepository.GetOrders(user.Id);

            UserOrdersVM userOrders = new UserOrdersVM
            {
                Orders = new List<OrderVM>()
            };


            foreach (Order o in orders)
            {
                
                OrderVM orderVm = new OrderVM();
                var DbOrderedOutfits = orderedOutfitRepository.GetOrderedOutfits(o.Id);

                foreach (OrderedOutfit ob in DbOrderedOutfits)
                {
                    var outfit = OutfitRepository.Get(ob.OutfitId);
                    int price = outfit.Price * ob.Quantity;
                    if (outfit.Discount.HasValue) 
                    {
                        price = price * (100 - outfit.Discount.Value) / 100;
                    }
                    
                    CartLine ct = new CartLine
                    {
                        Price = price,
                        Outfit = outfit,
                        Quantity = ob.Quantity
                    };
                    orderVm.Lines.Add(ct);
                    
                    

                }
                sum += o.Price;
                orderVm.Number = ++number;
                orderVm.OrderPrice = o.Price;
                orderVm.OrderId = o.Id;

                userOrders.Orders.Add(orderVm);

                

            }
            if (sort == "DSC")
            {
                userOrders.Orders.Reverse();
                TempData["Sort"] = "ASC";

            }
            else 
            {
                TempData["Sort"] = "DSC";
            }
            
            userOrders.Price = sum;
            return View(userOrders);
        }

        public CartIndexViewModel GetUserOrder(int orderId )
        {
            IList<OrderedOutfit> orderedOutfits = orderedOutfitRepository.GetOrderedOutfits(orderId);
            CartIndexViewModel cart = new CartIndexViewModel();
            cart.OrderId = orderId;
            cart.ReturnUrl = "null";

            List<CartLine> Lines = new List<CartLine>();
            foreach (OrderedOutfit ob in orderedOutfits)
            {
                CartLine ct = new CartLine
                {
                    Outfit = OutfitRepository.Get(ob.OutfitId),
                    Quantity = ob.Quantity
                };
                Lines.Add(ct);
            }
            cart.Cart.Lines = Lines;
            return cart;
        }



        [HttpGet]
        public IActionResult EditOrder(int orderId)
        {
            order = GetOrderEditor();
            if (order.Lines.Count == 0)
            {
                var model = GetUserOrder(orderId);
                foreach (CartLine ct in model.Cart.Lines) order.AddLine(ct.Outfit, ct.Quantity);
                SaveOrderEditor(order);
                return View(model);
            }
            else
            {
                Cart cart = new Cart
                {
                    Lines = order.Lines
                };
                CartIndexViewModel model = new CartIndexViewModel
                {
                    Cart = cart,
                    OrderId = orderId
                };
                
                return View(model);
            }
        }

        [HttpPost]
        public IActionResult SaveOrder(int orderId)
        {
            int sum = 0;
            foreach (OrderedOutfit ob in orderedOutfitRepository.GetOrderedOutfits(orderId))
            {
                var Outfit = OutfitRepository.Get(ob.OutfitId) ;
                sum += Outfit.Price * ob.Quantity;
            }
            Order newOrder = orderRepository.GetOrder(orderId);
            newOrder.Price = sum;
            orderRepository.EditOrder(newOrder);
           

            if (order.Lines == null)
            {
                orderRepository.RemoveOrder(orderId);
                orderedOutfitRepository.RemoveOrderedOutfits(orderId);
            }
            else
            {
                orderedOutfitRepository.RemoveOrderedOutfits(orderId);
                orderedOutfitRepository.AddOrderedOutfits(order.Lines, orderId);
            }
           


            return RedirectToAction("UserOrders");
        }

        //CHANGE LINE
        [HttpPost]
        public IActionResult ChangeLine(int OutfitId, int quantity, int orderId)
        {
            order.ChangeLine(OutfitId, quantity);
            TempData["message"] = "Changed";
            return RedirectToAction("EditOrder", new { orderId });
        }
        //DELETE LINE
        [HttpGet]
        public IActionResult RemoveFromOrder(int OutfitId, int orderId)
        {

            order.RemoveLine(OutfitId);
            return RedirectToAction("EditOrder", new { orderId });
        }


        [HttpGet]
        public IActionResult RemoveOrder(int orderId)
        {
            orderedOutfitRepository.RemoveOrderedOutfits(orderId);
            orderRepository.RemoveOrder(orderId);
            
            return RedirectToAction("UserOrders");
        }


        //Query
        [Authorize(Roles = "admin,moderator")]
        [HttpGet]
        public IActionResult PopularBuy(string returnUrl, SortState sortOrder = SortState.TitleAsc)
        {
            ViewBag.ReturnUrl = returnUrl;
            var Outfits = orderedOutfitRepository.PopularOutfits();

            switch (sortOrder)
            {
                case SortState.TitleDesc:
                    Outfits = Outfits.OrderByDescending(s => s.Outfit.Title);
                    break;
                case SortState.PriceAsc:
                    Outfits = Outfits.OrderBy(s => s.OutfitQuantity);
                    break;
                case SortState.PriceDesc:
                    Outfits = Outfits.OrderByDescending(s => s.OutfitQuantity);
                    break;

                default:
                    Outfits = Outfits.OrderBy(s => s.Outfit.Title);
                    break;
            }
            PopularOutfitsVm model = new PopularOutfitsVm
            {
                OrderedOutfits = Outfits,
                SortViewModel = new SortViewModel(sortOrder),
                ReturnUrl = returnUrl 
                
            };

            return View(model);
        }

        public IActionResult ConvertToPdf(string Url, string name)
        {
            HtmlToPdfConverter converter = new HtmlToPdfConverter();

            WebKitConverterSettings settings = new WebKitConverterSettings();

            settings.WebKitPath = Path.Combine(hostingEnvironment.ContentRootPath, "QtBinariesWindows");
            converter.ConverterSettings = settings;

            //
            PdfDocument doc = converter.Convert(string.Concat("https://localhost:44301", Url));

            MemoryStream ms = new MemoryStream();
            doc.Save(ms);
            doc.Close(true);

            ms.Position = 0;

            FileStreamResult fileStream = new FileStreamResult(ms, "application/pdf");
            fileStream.FileDownloadName = name;

            return fileStream;

        }

        
    }
}
