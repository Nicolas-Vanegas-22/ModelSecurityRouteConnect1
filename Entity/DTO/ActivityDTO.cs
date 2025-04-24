using System;

namespace Entity.DTO
{
    public class ActivityDTO
    {
        public int ActivityId { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public required string Category { get; set; }
        public decimal Price { get; set; }
    }

}