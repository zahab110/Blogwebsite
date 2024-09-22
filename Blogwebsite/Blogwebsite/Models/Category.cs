using System;
using System.Collections.Generic;

namespace Blogwebsite.Models;

public partial class Category
{
    public int CategoryId { get; set; }

    public string? CategoryName { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual ICollection<Blog> Blogs { get; set; } = new List<Blog>();
}
