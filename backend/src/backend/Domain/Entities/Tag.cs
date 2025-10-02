using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities;

public partial class Tag : BaseEntity<long>
{
    [StringLength(50)]
    public string Name { get; set; } = null!;

    [ForeignKey("TagId")]
    [InverseProperty("Tags")]
    public virtual ICollection<News> News { get; set; } = new List<News>();
}
