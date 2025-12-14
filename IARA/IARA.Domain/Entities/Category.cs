using System;
using System.Collections.Generic;

namespace IARA.Domain.Entities
{
    public class Category
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        
        public virtual ICollection<Product> Products { get; set; }
    }
}