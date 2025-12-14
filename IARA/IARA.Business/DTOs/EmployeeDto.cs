using System;

namespace IARA.Business.DTOs
{
    public class EmployeeDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Position { get; set; }
        public decimal MonthlySalary { get; set; }
        public DateTime EmploymentStartDate { get; set; }
        public Guid StoreId { get; set; }
        public string StoreName { get; set; }
    }

    public class CreateEmployeeDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Position { get; set; }
        public decimal MonthlySalary { get; set; }
        public DateTime EmploymentStartDate { get; set; }
        public Guid StoreId { get; set; }
    }
}