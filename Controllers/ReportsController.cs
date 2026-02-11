using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
// Reports using EPPlus and QuestPDF
using Visitapp.Data;
using Visitapp.Domain.Entities;

namespace Visitapp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly VisitAppContext _context;
        private readonly PdfReportService _pdfReportService;

        public ReportsController(VisitAppContext context)
        {
            _context = context;
            _pdfReportService = new PdfReportService();
        }
        // GET: api/reports/visits/pdf?startDate=2024-01-01&endDate=2024-12-31&userId=1
        [HttpGet("visits/pdf")]
        public async Task<IActionResult> ExportVisitsToPdf(DateTime? startDate, DateTime? endDate, int? userId)
        {
            var query = _context.Visits
                .Include(v => v.Contact)
                .Include(v => v.User)
                .AsQueryable();

            if (startDate.HasValue)
                query = query.Where(v => v.ScheduledDate >= startDate);
            if (endDate.HasValue)
                query = query.Where(v => v.ScheduledDate <= endDate);
            if (userId.HasValue)
                query = query.Where(v => v.UserId == userId);

            var visits = await query.ToListAsync();
            var pdfBytes = _pdfReportService.GenerateVisitsReport(visits);
            return File(pdfBytes, "application/pdf", "visitas.pdf");
        }

        // GET: api/reports/visits/excel?startDate=2024-01-01&endDate=2024-12-31&userId=1
        [HttpGet("visits/excel")]
        public async Task<IActionResult> ExportVisitsToExcel(DateTime? startDate, DateTime? endDate, int? userId)
        {
            var query = _context.Visits
                .Include(v => v.Contact)
                .Include(v => v.User)
                .AsQueryable();

            if (startDate.HasValue)
                query = query.Where(v => v.ScheduledDate >= startDate);
            if (endDate.HasValue)
                query = query.Where(v => v.ScheduledDate <= endDate);
            if (userId.HasValue)
                query = query.Where(v => v.UserId == userId);

            var visits = await query.ToListAsync();

            using var package = new ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("Visitas");
            ws.Cells[1, 1].Value = "ID";
            ws.Cells[1, 2].Value = "Usuario";
            ws.Cells[1, 3].Value = "Contacto";
            ws.Cells[1, 4].Value = "Fecha";
            ws.Cells[1, 5].Value = "Estado";
            ws.Cells[1, 6].Value = "Notas";

            for (int i = 0; i < visits.Count; i++)
            {
                var v = visits[i];
                ws.Cells[i + 2, 1].Value = v.VisitId;
                ws.Cells[i + 2, 2].Value = v.User?.FullName;
                ws.Cells[i + 2, 3].Value = v.Contact?.FullName;
                ws.Cells[i + 2, 4].Value = v.ScheduledDate.ToString("yyyy-MM-dd HH:mm");
                ws.Cells[i + 2, 5].Value = v.Status;
                ws.Cells[i + 2, 6].Value = v.Notes;
            }

            ws.Cells[ws.Dimension.Address].AutoFitColumns();
            var stream = new MemoryStream(package.GetAsByteArray());
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "visitas.xlsx");
        }
    }
}
