using Blogwebsite.Models;

namespace Blogwebsite.ViewModels
{
    public class IndexVM
    {
        public List<Category> AllCategories { get; set; }
        public List<Blog> Allblogs { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
    }
}
