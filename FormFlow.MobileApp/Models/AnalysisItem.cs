using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormFlow.MobileApp.Models;

public class AnalysisItem
{
    public required string Title { get; set; }
    public required string Date { get; set; }
    public required string Status { get; set; }
    public required Color StatusColor { get; set; }
    public required string ErrorCountText { get; set; }
}

