using System;

namespace Diplomka.Models
{
    public class EducationPrCode
    {
        public string? Id { get; set; }
        public string? CodeName { get; set; }
        public string? Description { get; set; }
        public EducationProgram EducationProgram { get; set; }
    }
}
