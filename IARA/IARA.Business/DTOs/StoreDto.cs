using System;

namespace IARA.Business.DTOs
{
    public class StoreDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Manager { get; set; }
        public string Region { get; set; }
        public string Territory { get; set; }
        public string District { get; set; }
        public string Settlement { get; set; }
    }

    public class CreateStoreDto
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Manager { get; set; }
        public string Region { get; set; }
        public string Territory { get; set; }
        public string District { get; set; }
        public string Settlement { get; set; }
    }
}