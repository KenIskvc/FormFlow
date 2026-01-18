using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormFlow.MobileApp.DTOs
{
    public class AnalysisResponseDto
    {
        public DateTime CreatedAt { get; set; }
        public int? AnalysisId { get; set; }
        public bool IsPersisted { get; set; }
        public int ErrorCount { get; set; }
        public string Report { get; set; } = string.Empty;
    }

}
