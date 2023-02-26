using System;
using System.Collections.Generic;

namespace Diplomka.Models
{
    public class EducationProgram
    {
        public string? Id { get; set; }
        public string? SubjId { get; set; }
        public string? EducationCodeId { get; set; }
        public EducationPrCode? EducationPrCode { get; set; }
        public List<Subjects> Subjects { get; set; } = new List<Subjects>();
    }
}
