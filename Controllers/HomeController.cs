using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SportStore.Models;
using Microsoft.AspNetCore.Hosting;
using SportStore.Models.Repositories;
using SportStore.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using SportStore.ViewModels.Feedback;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Net.Mail;
using System.Net;
using System.Diagnostics.Tracing;

namespace SportStore.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IOutfitRepository OutfitRepository;
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly UserManager<Customer> userManager;
        private readonly IDepartmentRepository departmentRepository;
        private readonly IFeedbackRepository feedbackRepository;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IOrderRepository orderRepository;
        private readonly IOrderedOutfitRepository orderedOutfitRepository;

        public HomeController(IOutfitRepository OutfitRepository, IHostingEnvironment hostingEnvironment, UserManager<Customer> userManager, IDepartmentRepository departmentRepository , 
            IFeedbackRepository feedbackRepository, IHttpContextAccessor HttpContextAccessor, IOrderRepository orderRepository,
                IOrderedOutfitRepository orderedOutfitRepository
            )
        {
            this.OutfitRepository = OutfitRepository;
            this.hostingEnvironment = hostingEnvironment;
            this.userManager = userManager;
            this.departmentRepository = departmentRepository;
            this.feedbackRepository = feedbackRepository;
            httpContextAccessor = HttpContextAccessor;
            this.orderRepository = orderRepository;
            this.orderedOutfitRepository = orderedOutfitRepository;
        }


      
        [AllowAnonymous]
        public IActionResult Index(string Outfittitle, string selectedDepartment = "Все", 
            int selectedMinPrice = 0, int selectedMaxPrice = 0,  string selectedSort = "По убыванию")
        {
            IEnumerable<Outfit> Outfits = OutfitRepository.GetAll();
           
            int minprice;
            int maxprice;
            List<int> discounts = new List<int>();
         
            ViewBag.Departments = new SelectList(departmentRepository.GetDepartments().Select(d => d.DepartmentName));
            

            if (Outfits.Any())
            {
                minprice = Outfits.Min(b => 
                {
                    if (b.Discount.HasValue) return b.Price * (100 - b.Discount.Value) / 100;
                    else return b.Price;
                });
                maxprice = Outfits.Max(b => 
                {
                    if (b.Discount.HasValue) return b.Price * (100 - b.Discount.Value) / 100;
                    else return b.Price;
                });

                if (selectedDepartment != null && selectedDepartment != "Все" && Outfits != null)
                {
                    Outfits = Outfits.Where(b => b.DepartmentName == selectedDepartment);
                }

                if (!string.IsNullOrEmpty(Outfittitle) && Outfits != null)
                {
                    Outfits = Outfits.Where(b => b.Title.ToLower().Contains(Outfittitle.ToLower()));
                }

                if (selectedMinPrice != 0 && Outfits != null)
                {
                    Outfits = Outfits.Where(b => b.Price >= selectedMinPrice);
                }
                if (selectedMaxPrice != 0 && Outfits != null)
                {
                    Outfits = Outfits.Where(b => b.Price <= selectedMaxPrice);
                }

            

                switch (selectedSort)
                {
                    case "По цене ↓":
                        Outfits = Outfits.OrderByDescending(b => b.Price);
                        break;
                    case "По алфавиту ↑":
                        Outfits = Outfits.OrderBy(b => b.Title);
                        break;
                    case "По алфавиту ↓":
                        Outfits = Outfits.OrderByDescending(b => b.Title);
                        break;
                    default:
                        Outfits = Outfits.OrderBy(b => b.Price);
                        break;
                }

                foreach (var i in Outfits)
                {
                    if (i.Discount.HasValue)
                    {
                        i.Discount = i.Price * (100 - i.Discount.Value) / 100;
                    }
                }
                var list = Outfits.ToList();
                

                IndexViewModel viewModel = new IndexViewModel
                {
                    Outfits = Outfits.ToList(),
                    SelectedDepartment = selectedDepartment,
                    SelectedMinPrice = selectedMinPrice,
                    SelectedMaxPrice = selectedMaxPrice,
                    MinPrice = minprice,
                    MaxPrice = maxprice,
                    OutfitTitle = Outfittitle,
                };
                return View(viewModel);
            }
            else
            {

                IndexViewModel viewModel = new IndexViewModel
                {
                    Outfits = Outfits.ToList(),
                    SelectedDepartment = selectedDepartment,
                    SelectedMinPrice = selectedMinPrice,
                    SelectedMaxPrice = selectedMaxPrice,
                    MinPrice = 0,
                    MaxPrice = 0,
                    OutfitTitle = Outfittitle,
                };
                return View(viewModel);
            }

            
        }

        int GetDiscount(int? discount)
        {
            if (discount.HasValue) {
                return (100 - discount.Value);
             }
            return 1;
        }
       

        public IActionResult Details(int OutfitId)
        {
            var Outfit = OutfitRepository.Get(OutfitId);
            var userId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            string userName = WindowsIdentity.GetCurrent().Name.ToString();
            List<FeedbackVM> feedModel = new List<FeedbackVM>(); 
            var feeds = feedbackRepository.GetFeedbacks(OutfitId);

            foreach (Feedback f in feeds)
            {
                FeedbackVM feedVM;

                if (f.CustomerId == userId)
                {
                    feedVM = new FeedbackVM
                    {
                        UserName = userName,
                        OutfitId = f.OutfitId,
                        Rating = f.Rating,
                        Comment = f.Comment,
                        CommentId = f.Id,
                        IsUsers = true
                    };
                }
                else
                {
                    feedVM = new FeedbackVM
                    {
                        UserName = userName,
                        OutfitId = f.OutfitId,
                        Rating = f.Rating,
                        Comment = f.Comment,
                        CommentId = f.Id,
                        IsUsers = false
                    };
                }
                feedModel.Add(feedVM);
                
            }
            double rating = feedbackRepository.OutfitRating(OutfitId);


            OutfitDetailsViewModel model = new OutfitDetailsViewModel
            {
                Feedbacks = feedModel,
                Outfit = Outfit,
                Rating = rating
            };
            return View(model);
            
        }
        

        


        public IActionResult AddToCart(int id)
        {
            var Outfit = OutfitRepository.Get(id);
            if (Outfit == null) { throw new Exception(); }
            OutfitRepository.AddToCart(Outfit);
            return RedirectToAction("Index");
        }

        public IActionResult Cart()
        {
            return View(OutfitRepository.GetCart());
        }

       


    }
}
