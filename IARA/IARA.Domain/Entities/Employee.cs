using System;
using System.Collections.Generic;

namespace IARA.Domain.Entities
{
    public class Employee
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Position { get; set; }
        public decimal MonthlySalary { get; set; }
        public DateTime EmploymentStartDate { get; set; }
        public Guid StoreId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        
        public virtual Store Store { get; set; }
        public virtual ICollection<Sale> Sales { get; set; }
    }
}