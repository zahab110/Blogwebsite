using Blogwebsite.Models;
using Blogwebsite.Repositories;
using Blogwebsite.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Blogwebsite.Services
{
    public class BlogService : IBlog
    {
        private readonly BlogWebsiteContext context;

        public BlogService(BlogWebsiteContext _context)
        {
            context = _context;
        }
        public async Task<List<Blog>> GetAllBlogs()
        {
            return await context.Blogs.Where(x => x.IsDeleted == false || x.IsDeleted == null).ToListAsync();
        }
        public async Task<int> GetTotalBlogCount()
        {
            return await context.Blogs
                .Where(x => x.IsDeleted == false || x.IsDeleted == null)
                .CountAsync();
        }
        public async Task<List<Blog>> GetAllIndexBlogs(int page, int pageSize)
        {
            return await context.Blogs
                .Where(x => x.IsDeleted == false || x.IsDeleted == null)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
        public async Task<Blog> AddBlog(BlogsVM blogVM, List<string> Tags)
        {
            var blog = new Blog
            {
                Title = blogVM.Title,
                Content = blogVM.Content,
                CategoryId = blogVM.CategoryId,
                Tags = blogVM.Tags = string.Join(",", Tags),
                CreatedDate = DateTime.Now
            };

            context.Blogs.Add(blog);
            int GetSavedCount = await context.SaveChangesAsync();
            if (GetSavedCount > 0)
            {
                return blog;
            }
            else
            {
                return null;
            }
        }
        public async Task<Blog> DeleteBlog(int ID)
        {
            var DeleteBlog = await context.Blogs.Where(x => x.PostId == ID).FirstOrDefaultAsync();
            if (DeleteBlog != null)
            {
                DeleteBlog.IsDeleted = true;
                int deleteCount = await context.SaveChangesAsync();
                if (deleteCount > 0)
                {
                    return DeleteBlog;
                }
            }
            return DeleteBlog;
        }
        public async Task<Blog> UpdateBlog(int ID)
        {
            return await context.Blogs.Where(x => x.PostId == ID).FirstOrDefaultAsync();
        }
        public async Task<Blog> SaveUpdatedBlog(BlogsVM blogVM, List<string> Tags)
        {
            var existingBlog = await context.Blogs.FirstOrDefaultAsync(b => b.PostId == blogVM.PostID);
            if (existingBlog != null)
            {
                existingBlog.Title = blogVM.Title;
                existingBlog.Content = blogVM.Content;
                existingBlog.CategoryId = blogVM.CategoryId;
                existingBlog.Tags = string.Join(",", Tags);
                existingBlog.UpdatedDate = DateTime.Now;

                context.Blogs.Update(existingBlog);

                int GetSavedCount = await context.SaveChangesAsync();
                if (GetSavedCount > 0)
                {
                    return existingBlog;
                }
            }
            return null;
        }
        public async Task<(List<Blog>, int)> GetSortFilteredData(string GetText, int page, int pageSize)
        {
            IQueryable<Blog> query = context.Blogs.Where(x => x.IsDeleted == false || x.IsDeleted == null);

            if (GetText == "Newest First")
            {
                query = query.OrderByDescending(x => x.CreatedDate);
            }
            else
            {
                query = query.OrderBy(x => x.CreatedDate);
            }

            var totalCount = await query.CountAsync();
            var blogs = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (blogs, totalCount);
        }

        public async Task<(List<Blog>, int)> GetDateSortingBlog(DateTime fromDate, DateTime toDate, int page, int pageSize)
        {
            toDate = toDate.Date.AddDays(1).AddTicks(-1);
            var query = context.Blogs.Where(x => x.CreatedDate >= fromDate && x.CreatedDate <= toDate && x.IsDeleted == false);

            var totalCount = await query.CountAsync();
            var blogs = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (blogs, totalCount);
        }

        public async Task<(List<Blog> blogs, int totalCount)> GetBlogsByCategory(int categoryId, int page, int pageSize)
        {
            IQueryable<Blog> query;

            if (categoryId == 0)
            {
                query = context.Blogs.Where(x => x.CategoryId != null && x.IsDeleted != true);
            }
            else
            {
                query = context.Blogs.Where(x => x.CategoryId == categoryId && x.CategoryId != null && x.IsDeleted != true);
            }

            var totalCount = await query.CountAsync();
            var blogs = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (blogs, totalCount);
        }

        public async Task<(List<Blog> blogs, int totalCount)> GetSortingByPostID(int postID, int page, int pageSize)
        {
            IQueryable<Blog> query;

            query = context.Blogs.Where(x => x.PostId == postID && x.CategoryId != null && x.IsDeleted != true);

            var totalCount = await query.CountAsync();
            var blogs = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (blogs, totalCount);
        }

        public async Task<List<Category>> GetAllCategory()
        {
            return await context.Categories.Where(x => x.IsDeleted == false || x.IsDeleted == null).ToListAsync();
        }
        public async Task<Category> DeleteCategory(int ID)
        {
            var DeleteCategory = await context.Categories.Where(x => x.CategoryId == ID).FirstOrDefaultAsync();
            if (DeleteCategory != null)
            {
                var blogsToUpdate = await context.Blogs.Where(b => b.CategoryId == ID).ToListAsync();
                foreach (var blog in blogsToUpdate)
                {
                    blog.CategoryId = null;
                }
                DeleteCategory.IsDeleted = true;
                int deleteCount = await context.SaveChangesAsync();
                if (deleteCount > 0)
                {
                    return DeleteCategory;
                }
            }
            return DeleteCategory;
        }
        public async Task<Category> UpdateCategory(int ID)
        {
            return await context.Categories.Where(x => x.CategoryId == ID).FirstOrDefaultAsync();
        }

        public async Task<Category> SaveUpdatedCategory(CategoryVM categoryVM)
        {
            var GetExistingCategory = await context.Categories.Where(x => x.CategoryId == categoryVM.CategoryID).FirstOrDefaultAsync();
            if (GetExistingCategory != null)
            {
                GetExistingCategory.CategoryName = categoryVM.CategoryName;
                int savedCount = await context.SaveChangesAsync();
                if (savedCount > 0)
                {
                    return GetExistingCategory;
                }
            }
            return GetExistingCategory;
        }
        public async Task<Category> AddCategory(CategoryVM categoryVM)
        {
            bool categoryExists = await context.Categories
           .AnyAsync(x => x.CategoryName.ToLower() == categoryVM.CategoryName.ToLower());

            if (categoryExists)
            {
                categoryVM.IsExist = true;
                return null; 

            }

            var category = new Category
            {
                CategoryName = categoryVM.CategoryName,
            };

            context.Categories.Add(category);
            int GetSavedCount = await context.SaveChangesAsync();
            if (GetSavedCount > 0)
            {
                return category;
            }

            return null;
        }

    }
}
