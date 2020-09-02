using SportStore.Models;
using SportStore.Models.Repositories;
using SportStore.TagHelpers;
using SportStore.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SportStore.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace SportStore.Controllers
{

    [Authorize(Roles = "admin,moderator")]
    public class ModerationController : Controller
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly UserManager<Customer> userManager;
        private readonly IOrderedOutfitRepository orderedOutfitRepository;
        private readonly IFeedbackRepository feedbackRepository;

        public readonly IOutfitRepository OutfitRepository;
        public IHostingEnvironment HostingEnvironment;
        private readonly IDepartmentRepository departmentRepository;

        public ModerationController(IOutfitRepository outfitRepository, IHostingEnvironment hostingEnvironment, IDepartmentRepository departmentRepository,
            IHttpContextAccessor httpContextAccessor ,UserManager<Customer> userManager,IOrderedOutfitRepository orderedOutfitRepository, IFeedbackRepository feedbackRepository)
        {
            OutfitRepository = outfitRepository;
            HostingEnvironment = hostingEnvironment;
            this.departmentRepository = departmentRepository;
            this.httpContextAccessor = httpContextAccessor;
            this.userManager = userManager;
            this.orderedOutfitRepository = orderedOutfitRepository;
            this.feedbackRepository = feedbackRepository;
        }


        //INDEX
        public IActionResult Outfits(string Title, string selectedDepartment = "Все",
            int selectedPrice = 0, SortState sortOrder = SortState.TitleAsc)
        {
            List<Outfit> DbOutfits = new List<Outfit>();
            int minprice = 0;
            int maxprice = 0;
            ViewBag.Departments = new SelectList(departmentRepository.GetDepartments().Select(d => d.DepartmentName));

            if (!OutfitRepository.IsEmpty())
            {
                var Outfits = OutfitRepository.GetAll();
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

                if (selectedDepartment != null && selectedDepartment != "Все")
                {
                    Outfits = Outfits.Where(b => b.DepartmentName == selectedDepartment);
                }


                if (selectedPrice != 0)
                {
                    Outfits = Outfits.Where(b => b.Price >= selectedPrice);
                }

                if (!string.IsNullOrEmpty(Title))
                {
                    Outfits = Outfits.Where(b => b.Title.ToLower().Contains(Title.ToLower()));
                }

            

                switch (sortOrder)
                {
                    case SortState.TitleDesc:
                        Outfits = Outfits.OrderByDescending(s => s.Title);
                        break;
                    case SortState.PriceAsc:
                        Outfits = Outfits.OrderBy(s => s.Price);
                        break;
                    case SortState.PriceDesc:
                        Outfits = Outfits.OrderByDescending(s => s.Price);
                        break;
                    case SortState.IdAsc:
                        Outfits = Outfits.OrderBy(s => s.Id);
                        break;
                    case SortState.IdDesc:
                        Outfits = Outfits.OrderByDescending(s => s.Id);
                        break;
                    default:
                        Outfits = Outfits.OrderBy(s => s.Title);
                        break;
                }
                DbOutfits = Outfits.ToList();


              
            }

            OutfitsVM viewModel = new OutfitsVM
            {
                SelectedDepartment = selectedDepartment,
                SelectedPrice = selectedPrice,
                MaxPrice = maxprice,
                MinPrice = minprice,
                Outfits = DbOutfits,
                SortViewModel = new SortViewModel(sortOrder)
            };
            
            return View(viewModel);
        }



        //CREATE
        [HttpGet]
        public IActionResult CreateOutfit()
        {
            ViewBag.Departments = new SelectList(departmentRepository.GetDepartments().Select(d => d.DepartmentName));

            return View();
        }

        [HttpPost]
        public ActionResult CreateOutfit(OutfitCreate model)
        {
            if (ModelState.IsValid)
            {

                string UniqueFilePath = ProcessUploadedFile(model);
                Outfit Outfit = new Outfit
                {
                    Title = model.Title,
                    Price = model.Price,
                    DepartmentName = model.DepartmentName,
                    Description = model.Description,
                    PhotoPath = UniqueFilePath,
                    Discount = model.Discount
                };
                OutfitRepository.Add(Outfit);
                TempData["message"] = $"{Outfit.Title} has been added";

            }
            else
            {

                ViewBag.Departments = new SelectList(departmentRepository.GetDepartments().Select(d => d.DepartmentName));
                return View(model);
            }


            return RedirectToAction("Outfits");
        }

        //EDIT
        [HttpGet]
        public ActionResult EditOutfit(int id)
        {
            var departments = new SelectList(departmentRepository.GetDepartments().Select(d => d.DepartmentName));

            var Outfit = OutfitRepository.Get(id);
            OutfitEdit model = new OutfitEdit
            {
                Id = Outfit.Id,
                Title = Outfit.Title,
                Price = Outfit.Price,
                Discount = Outfit.Discount,
                DepartmentName = Outfit.DepartmentName,
                Description = Outfit.Description,
                ExistingPhotoPath = Outfit.PhotoPath
            };

            foreach (var d in departments)
            {
                if (d.Value == model.DepartmentName) 
                {
                    d.Selected = true;
                    break;
                }
            }

            ViewBag.Department = departments;

            return View(model);
        }

        [HttpPost]
        public ActionResult EditOutfit(OutfitEdit model)
        {
            if (ModelState.IsValid)
            {
                Outfit Outfit = new Outfit
                {
                    Id = model.Id,
                    Title = model.Title,
                    Price = model.Price,
                    Discount = model.Discount,
                    Description = model.Description,
                    DepartmentName = model.DepartmentName,
                    PhotoPath = model.ExistingPhotoPath
                };

                if (model.Photo != null)
                {
                    if (model.ExistingPhotoPath != null)
                    {
                        string FilePath = Path.Combine(HostingEnvironment.WebRootPath, "images", model.ExistingPhotoPath);
                        System.IO.File.Delete(FilePath);
                    }
                    Outfit.PhotoPath = ProcessUploadedFile(model);
                }

                OutfitRepository.Edit(Outfit);
                TempData["message"] = $"{Outfit.Title} has been edited";


            }
            else 
            {
                ViewBag.Departments = new SelectList(departmentRepository.GetDepartments().Select(d => d.DepartmentName));
                return View(model) ;
            }
            return RedirectToAction("Outfits");
        }

        //UNIQUE NAME
        private string ProcessUploadedFile(OutfitCreate model)
        {
            string UniqueFileName = null;
            if (model.Photo != null)
            {
                var uploadFolder = Path.Combine(HostingEnvironment.WebRootPath, "images");
                UniqueFileName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
                string FilePath = Path.Combine(uploadFolder, UniqueFileName);
                model.Photo.CopyTo(new FileStream(FilePath, FileMode.Create));
            }

            return UniqueFileName;
        }


        //DETAILS
        public ActionResult DetailsOutfit(int id)
        {
            var Outfit = OutfitRepository.Get(id);
            return View(Outfit);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var Outfit = OutfitRepository.Get(id);

            feedbackRepository.RemoveOutfitFeedback(id);
            orderedOutfitRepository.RemoveOrdersByOutfits(id);
            OutfitRepository.Remove(Outfit);

            TempData["message"] = $"{Outfit.Title} has been deleted";
            return RedirectToAction("Outfits");
        }

        [HttpPost]
        public IActionResult AddDepartment(string dept) 
        {
            if (departmentRepository.AddDepartment(dept))
            {
                TempData["message"] = "Роздел успешно добавлен";
            }
            else
                TempData["BadMessage"] = "Произошла ошибка, возможно раздел уже существует";

            return RedirectToAction("Outfits");
        }


        [HttpPost]
        public IActionResult RemoveDepartment(string dept)
        {
            if (departmentRepository.RemoveDepartment(dept))
            {
                TempData["message"] = "Роздел успешно удален";
            }
            else
                TempData["BadMessage"] = "Произошла ошибка, возможно такого раздела не существует";

            return RedirectToAction("Outfits");
        }




    }
}
