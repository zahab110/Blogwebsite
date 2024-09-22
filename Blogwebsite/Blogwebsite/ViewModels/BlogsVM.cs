using System;
using System.ComponentModel.DataAnnotations;

namespace Blogwebsite.ViewModels
{
    public class BlogsVM
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title cannot be longer than 100 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Content is required")]
        public string Content { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public int CategoryId { get; set; }
        public int PostID { get; set; }

        [Required(ErrorMessage = "Tags are required")]
        [RegularExpression(@"^(\w+)(,\s*\w+)*$", ErrorMessage = "Tags should be comma-separated words.")]
        public string Tags { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }
    }
}
