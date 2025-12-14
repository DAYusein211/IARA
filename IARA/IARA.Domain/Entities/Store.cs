using System;
using System.Collections.Generic;

namespace IARA.Domain.Entities
{
    public class Store
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Manager { get; set; }
        public string Region { get; set; }
        public string Territory { get; set; }
        public string District { get; set; }
        public string Settlement { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        
        public virtual ICollection<Employee> Employees { get; set; }
        public virtual ICollection<Sale> Sales { get; set; }
    }
}