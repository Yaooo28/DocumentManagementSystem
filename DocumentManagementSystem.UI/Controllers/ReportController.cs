using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.IO;  // For MemoryStream
using OfficeOpenXml;  // EPPlus for Excel generation
using DocumentManagementSystem.DataAccess.Contexts;  // Updated namespace for DocumentContext
using DocumentManagementSystem.Entities;  // Updated namespace for Document entity

namespace DocumentManagementSystem.UI.Controllers  // Adjust based on your folder structure
{
    public class ReportController : Controller
    {
        private readonly DocumentContext _context;

        public ReportController(DocumentContext context)
        {
            _context = context;
        }

        // Action to display the report page
        public IActionResult Index()
        {
            var documents = _context.Documents.ToList();  // Fetch data from the database
            return View(documents);  // Pass documents to the view
        }

        // Action to show details for a specific document
        public IActionResult Details(int id)
        {
            var document = _context.Documents.FirstOrDefault(d => d.Id == id);
            if (document == null)
            {
                return NotFound();
            }
            return View(document);  // Pass the document to the Details view
        }

        // Action to export document details to Excel
        public IActionResult ExportDetails(int id)
        {
            var document = _context.Documents.FirstOrDefault(d => d.Id == id);
            if (document == null)
            {
                return NotFound();
            }

            // Create a new Excel package using EPPlus
            using (var package = new ExcelPackage())
            {
                // Add a worksheet to the package
                var worksheet = package.Workbook.Worksheets.Add("DocumentDetails");

                // Add headers in the first row
                worksheet.Cells[1, 1].Value = "Field";
                worksheet.Cells[1, 2].Value = "Value";

                // Add document data to the worksheet
                worksheet.Cells[2, 1].Value = "Title";
                worksheet.Cells[2, 2].Value = document.Title;

                worksheet.Cells[3, 1].Value = "Type";
                worksheet.Cells[3, 2].Value = document.TypeOfDoc;

                worksheet.Cells[4, 1].Value = "Sender";
                worksheet.Cells[4, 2].Value = document.SenderName;

                worksheet.Cells[5, 1].Value = "Receiver";
                worksheet.Cells[5, 2].Value = document.ReceiverName;

                worksheet.Cells[6, 1].Value = "Created Date";
                worksheet.Cells[6, 2].Value = document.CreatedDate.ToString("MM/dd/yyyy h:mm tt");

                // Auto-fit columns
                worksheet.Cells.AutoFitColumns();

                // Create a memory stream to save the Excel package
                var stream = new MemoryStream();
                package.SaveAs(stream);

                // Set the stream position back to 0 before returning the file
                stream.Position = 0;

                // Return the Excel file to the client
                var fileName = $"DocumentDetails_{document.Id}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }
    }
}
