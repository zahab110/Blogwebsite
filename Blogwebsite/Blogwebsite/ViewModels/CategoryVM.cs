using System.ComponentModel.DataAnnotations;

namespace Blogwebsite.ViewModels
{
    public class CategoryVM
    {
        [Required(ErrorMessage = "Category name is required.")]
        public string CategoryName { get; set; }
        public int CategoryID { get; set; }
        public bool IsExist { get; set; }
    }
}
