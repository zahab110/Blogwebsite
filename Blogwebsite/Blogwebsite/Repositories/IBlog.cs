using Blogwebsite.Models;
using Blogwebsite.ViewModels;

namespace Blogwebsite.Repositories
{
    public interface IBlog
    {
        public Task<List<Blog>> GetAllBlogs();
        public Task<List<Blog>> GetAllIndexBlogs(int page, int pageSize);
        public Task<int> GetTotalBlogCount();
        public Task<Blog> DeleteBlog(int ID);
        public Task<Blog> UpdateBlog(int ID);
        public Task<Blog> AddBlog(BlogsVM blog, List<string> Tags);
        public Task<Blog> SaveUpdatedBlog(BlogsVM blog, List<string> Tags);
        public Task<(List<Blog>, int)> GetSortFilteredData(string GetText, int page, int pageSize);
        public Task<(List<Blog> blogs, int totalCount)> GetSortingByPostID(int postID, int page, int pageSize);
        public Task<(List<Blog> blogs, int totalCount)> GetBlogsByCategory(int categoryId, int page, int pageSize);
        public Task<(List<Blog>, int)> GetDateSortingBlog(DateTime fromDate, DateTime toDate, int page, int pageSize);
        public Task<List<Category>> GetAllCategory();
        public Task<Category> DeleteCategory(int ID);
        public Task<Category> UpdateCategory(int ID);
        public Task<Category> SaveUpdatedCategory(CategoryVM category);
        public Task<Category> AddCategory(CategoryVM category);

    }
}
