using Blogwebsite.Models;
using Blogwebsite.Repositories;
using Blogwebsite.Services;
using Blogwebsite.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;
using System.Reflection.Metadata;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Blogwebsite.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBlog _blogService;
        public HomeController(IBlog blogService)
        {
            _blogService = blogService;
        }
        public async Task<IActionResult> Index(int page = 1)
        {
            int pageSize = 2;

            var GetAllBlogs = await _blogService.GetAllIndexBlogs(page, pageSize);
            if (GetAllBlogs.Count == 0)
            {
                ViewBag.BlogMessage = "No Blogs Available";
            }

            var GetAllCategories = await _blogService.GetAllCategory();
            if (GetAllCategories.Count == 0)
            {
                ViewBag.CategoryMessage = "No Categories Available";
            }

            var totalBlogs = await _blogService.GetTotalBlogCount();
            var totalPages = (int)Math.Ceiling((double)totalBlogs / pageSize);

            var viewModel = new IndexVM
            {
                Allblogs = GetAllBlogs,
                AllCategories = GetAllCategories,
                CurrentPage = page,
                TotalPages = totalPages
            };

            return View(viewModel);
        }

        public async Task<IActionResult> AddCategory()
        {
            ViewBag.Title = "Add Category";
            return View();
        }

        public async Task<IActionResult> AddBlog()
        {
            ViewBag.Title = "Add Blog";

            var GetAllCategories = await _blogService.GetAllCategory();
            ViewBag.GetAllCategories = GetAllCategories.Select(x => new SelectListItem
            {
                Value = x.CategoryId.ToString(),
                Text = x.CategoryName
            });
            return View();
        }

        public async Task<IActionResult> GetSortingBlog(string GetText, int page = 1, int pageSize = 2)
        {
            if (!string.IsNullOrEmpty(GetText))
            {
                var (data, totalCount) = await _blogService.GetSortFilteredData(GetText, page, pageSize);
                if (data != null)
                {
                    var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
                    return Json(new { data, currentPage = page, totalPages, message = "Successfully Fetched...", success = true });
                }
                return Json(new { message = "No Data To Fetch", success = false });
            }
            return Json(new { message = "Something Went Wrong...", success = false });
        }

        public async Task<IActionResult> GetSortingByPostID(int GetID, int page = 1, int pageSize = 2)
        {
            if (GetID <= 0) {
                return Json(new { message = "No Data On The Given PostID", success = false });
            }
            var (data, totalCount) = await _blogService.GetSortingByPostID(GetID, page, pageSize);
            if (data != null)
            {
                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
                return Json(new { data, currentPage = page, totalPages, message = "Successfully Fetched...", success = true });
            }
            return Json(new { message = "No Data To Fetch", success = false });
        }

        public async Task<IActionResult> GetDateSortingBlog(DateTime fromDate, DateTime toDate, int page = 1, int pageSize = 2)
        {
            var (data, totalCount) = await _blogService.GetDateSortingBlog(fromDate, toDate, page, pageSize);
            if (data != null)
            {
                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
                return Json(new { data, currentPage = page, totalPages, message = "Successfully Fetched...", success = true });
            }
            return Json(new { message = "Something Went Wrong...", success = false });
        }
        public async Task<IActionResult> GetBlogsByCategory(int categoryId, int page = 1, int pageSize = 2)
        {
            var (blogs, totalCount) = await _blogService.GetBlogsByCategory(categoryId, page, pageSize);
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            return Json(new
            {
                data = blogs,
                currentPage = page,
                totalPages = totalPages,
                message = "Successfully Fetched...",
                success = true
            });
        }

        [HttpPost]
        public async Task<IActionResult> AddBlog(BlogsVM blog, List<string> Tags)
        {
            if (blog.CategoryId == 0)
            {
                var GetAllCategories = await _blogService.GetAllCategory();
                ViewBag.GetAllCategories = GetAllCategories.Select(x => new SelectListItem
                {
                    Value = x.CategoryId.ToString(),
                    Text = x.CategoryName
                });
                TempData["ErrorMessage"] = "Please Select At Least One Category";

                return View(blog);
            }
            if (ModelState.IsValid)
            {
                var saveBlog = await _blogService.AddBlog(blog, Tags);
                if (saveBlog != null)
                {
                    TempData["SuccessMessage"] = "Blog Uploaded Successfully";
                }
                return RedirectToAction(nameof(AllBlogs));
            }
            return View(blog);
        }

        public async Task<IActionResult> AllBlogs()
        {
            var GetAllBlogs = await _blogService.GetAllBlogs();
            if (GetAllBlogs.Count == 0)
            {
                ViewBag.BlogMessage = "No Blogs Available";
            }
            return View(GetAllBlogs);
        }

        public async Task<IActionResult> UpdateBlog(int id)
        {
            var data = await _blogService.UpdateBlog(id);
            var allCategories = await _blogService.GetAllCategory();
            ViewBag.GetAllCategories = new SelectList(allCategories, "CategoryId", "CategoryName", data.CategoryId);
            return View(data);
        }

        public async Task<IActionResult> DeleteBlog(int id)
        {
            var data = await _blogService.DeleteBlog(id);
            if (data != null)
            {
                TempData["DeleteMessage"] = "Blog Deleted Successfully";
                return RedirectToAction(nameof(AllBlogs));
            }
            return RedirectToAction(nameof(AllBlogs));
        }

        public async Task<IActionResult> SaveUpdatedBlog(BlogsVM blog, List<string> Tags)
        {
            if (ModelState.IsValid)
            {
                var saveBlog = await _blogService.SaveUpdatedBlog(blog, Tags);
                if (saveBlog != null)
                {
                    ViewBag.SuccessMessage = "Blog Updated Saved Successfully";
                }
                return RedirectToAction(nameof(AllBlogs));
            }
            return View(blog);
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory(CategoryVM category)
        {
            if (ModelState.IsValid)
            {
                var data = await _blogService.AddCategory(category);
                if (category.IsExist)
                {
                    ViewBag.ErrorMessage = "Category already exists. Please use a different name.";
                }
                else if (data != null)
                {
                    ViewBag.SuccessMessage = "Category Added Successfully.";
                }
            }
            return View(category);
        }

        public async Task<IActionResult> AllCategory()
        {
            var data = await _blogService.GetAllCategory();
            return View(data);
        }

        public async Task<IActionResult> UpdateCategory(int id)
        {
            var data = await _blogService.UpdateCategory(id);
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> SaveUpdatedCategory(CategoryVM category)
        {
            var data = await _blogService.SaveUpdatedCategory(category);
            return RedirectToAction(nameof(AllCategory));
        }
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var data = await _blogService.DeleteCategory(id);
            if (data != null)
            {
                TempData["SuccessMessage"] = "Category Deleted Successfully...";
                return RedirectToAction(nameof(AllCategory));
            }
            return RedirectToAction(nameof(AllCategory));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
