using SportStore.Models;
using SportStore.Models.Repositories;
using SportStore.TagHelpers;
using SportStore.ViewModels;
using SportStore.ViewModels.Feedback;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SportStore.Controllers
{
    [Authorize]
    public class FeedbackController : Controller
    {
        private IFeedbackRepository feedbackRepository;
        private IOutfitRepository OutfitRepository;
        private readonly IHttpContextAccessor httpContextAccessor;

        public FeedbackController(IFeedbackRepository feedbackRepository, IOutfitRepository OutfitRepository, IHttpContextAccessor httpContextAccessor)
        {
            this.feedbackRepository = feedbackRepository;
            this.OutfitRepository = OutfitRepository;
            this.httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public IActionResult CreateFeedback(int OutfitId)
        {
            ViewData["OutfitId"] = OutfitId;
            return View();
        }

        [HttpPost]
        public IActionResult CreateFeedback(FeedbackVM model)
        {
            if (ModelState.IsValid)
            {
                var userId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                Feedback feedback = new Feedback
                {
                    Comment = model.Comment,
                    Rating = model.Rating,
                    OutfitId = model.OutfitId,
                    CustomerId = userId,
                    Date = DateTime.Now
                };
                feedbackRepository.AddFeedback(feedback);
            }
            return RedirectToAction("Details", "Home", new { model.OutfitId });
        }

        public IActionResult RemoveFeedback(int feedbackId, int OutfitId)
        {
            feedbackRepository.RemoveFeedback(feedbackId);
            return RedirectToAction("Details", "Home", new { OutfitId }); 
        }

        [Authorize(Roles = "admin,moderator")]
        public IActionResult BestRatedOutfits(string returnUrl, SortState sortOrder = SortState.TitleAsc)
        {
            ViewBag.ReturnUrl = returnUrl;
            var Outfits = feedbackRepository.BestRatedOutfits().AsEnumerable();
            switch (sortOrder)
            {
                case SortState.TitleDesc:
                    Outfits =  Outfits.OrderByDescending(s => s.Outfit.Title);
                    break;
                case SortState.PriceAsc:
                    Outfits = Outfits.OrderBy(s => s.Rating);
                    break;
                case SortState.PriceDesc:
                    Outfits = Outfits.OrderByDescending(s => s.Rating);
                    break;

                default:
                    Outfits = Outfits.OrderBy(s => s.Outfit.Title);
                    break;
            }
            TopRatedOutfitsVM model = new TopRatedOutfitsVM
            {
                RatedOutfits = Outfits.AsEnumerable(),
                SortViewModel = new SortViewModel(sortOrder),
                ReturnUrl = returnUrl

            };
            
            return View(model);


           
        }

        [Authorize(Roles = "admin,moderator")]
        public IActionResult BestInGenre(string returnUrl, SortState sortOrder = SortState.TitleAsc)
        {

            ViewBag.ReturnUrl = returnUrl;
            var Outfits = feedbackRepository.BestInGenre();
            switch (sortOrder)
            {
                case SortState.TitleDesc:
                    Outfits = Outfits.OrderByDescending(s => s.Title);
                    break;
                case SortState.PriceAsc:
                    Outfits = Outfits.OrderBy(s => s.Rating);
                    break;
                case SortState.PriceDesc:
                    Outfits = Outfits.OrderByDescending(s => s.Rating);
                    break;

                default:
                    Outfits = Outfits.OrderBy(s => s.Title);
                    break;
            }
            BestInGenreVM model = new BestInGenreVM
            {
                Outfits = Outfits,
                SortModel = new SortViewModel(sortOrder),
                ReturnUrl = returnUrl

            };

            return View(model);
        }
    }
}
