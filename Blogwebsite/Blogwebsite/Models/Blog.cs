using System;
using System.Collections.Generic;

namespace Blogwebsite.Models;

public partial class Blog
{
    public int PostId { get; set; }

    public string? Title { get; set; }

    public string? Content { get; set; }

    public int? CategoryId { get; set; }

    public string? Tags { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual Category? Category { get; set; }
}
