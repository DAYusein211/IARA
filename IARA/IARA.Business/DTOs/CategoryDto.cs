using System;

namespace IARA.Business.DTOs
{
    public class CategoryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class CreateCategoryDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}