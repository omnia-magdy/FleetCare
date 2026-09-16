using FleetCarePro.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FleetCarePro.ViewComponents;

public class FleetCostSummaryViewComponent : ViewComponent
{
    private readonly ApplicationDbContext _context;

    public FleetCostSummaryViewComponent(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        DateTime now = DateTime.Today;
        DateTime startOfMonth = new DateTime(now.Year, now.Month, 1);
        DateTime endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

        decimal totalMonthlyCost = await _context.ServiceRecords
            .Where(sr => sr.ServiceDate >= startOfMonth && sr.ServiceDate <= endOfMonth)
            .SumAsync(sr => (decimal?)sr.TotalCost) ?? 0;

        int totalInvoicesCount = await _context.ServiceRecords
            .Where(sr => sr.ServiceDate >= startOfMonth && sr.ServiceDate <= endOfMonth)
            .CountAsync();

        var model = new FleetCostSummaryViewModel
        {
            TotalCost = totalMonthlyCost,
            InvoicesCount = totalInvoicesCount,
            MonthName = now.ToString("MMMM yyyy")
        };

        return View(model);
    }
}

public class FleetCostSummaryViewModel
{
    public decimal TotalCost { get; set; }
    public int InvoicesCount { get; set; }
    public string MonthName { get; set; } = string.Empty;
}