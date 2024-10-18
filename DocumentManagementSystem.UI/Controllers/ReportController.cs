using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using DocumentManagementSystem.DataAccess.Contexts;
using DocumentManagementSystem.Entities;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using WordDocument = DocumentFormat.OpenXml.Wordprocessing.Document;
using WordTable = DocumentFormat.OpenXml.Wordprocessing.Table;
using WordFontSize = DocumentFormat.OpenXml.Wordprocessing.FontSize;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace DocumentManagementSystem.UI.Controllers
{
    public class ReportController : Controller
    {
        private readonly DocumentContext _context;

        public ReportController(DocumentContext context)
        {
            _context = context;
        }

        // Action to display the report page with search and filters
        public IActionResult Index(string searchString, DateTime? startDate, DateTime? endDate, string reportType)
        {
            var documents = _context.Documents.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                documents = documents.Where(d => d.Title.Contains(searchString) || d.SenderName.Contains(searchString) || d.ReceiverName.Contains(searchString));
            }

            if (startDate.HasValue)
            {
                documents = documents.Where(d => d.CreatedDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                documents = documents.Where(d => d.CreatedDate <= endDate.Value);
            }

            if (!string.IsNullOrEmpty(reportType))
            {
                DateTime currentDate = DateTime.Now;

                switch (reportType)
                {
                    case "Daily":
                        documents = documents.Where(d => d.CreatedDate.Date == currentDate.Date);
                        break;
                    case "Weekly":
                        var currentWeekStart = StartOfWeek(currentDate, DayOfWeek.Monday);
                        var currentWeekEnd = currentWeekStart.AddDays(7);
                        documents = documents.Where(d => d.CreatedDate >= currentWeekStart && d.CreatedDate < currentWeekEnd);
                        break;
                    case "Monthly":
                        var currentMonthStart = new DateTime(currentDate.Year, currentDate.Month, 1);
                        var currentMonthEnd = currentMonthStart.AddMonths(1);
                        documents = documents.Where(d => d.CreatedDate >= currentMonthStart && d.CreatedDate < currentMonthEnd);
                        break;
                }
            }

            ViewBag.CurrentFilter = searchString;
            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");
            ViewBag.ReportType = reportType;

            return View(documents.ToList());
        }

        private DateTime StartOfWeek(DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = dt.DayOfWeek - startOfWeek;
            if (diff < 0)
                diff += 7;
            return dt.AddDays(-1 * diff).Date;
        }

        public IActionResult ExportDetailsToWord(int id)
        {
            var document = _context.Documents.FirstOrDefault(d => d.Id == id);
            if (document == null)
            {
                return NotFound();
            }

            return GenerateWordDocument(new List<DocumentManagementSystem.Entities.Document> { document }, $"DocumentDetails_{document.Id}.docx");
        }

        public IActionResult ExportAllToWord(string searchString, DateTime? startDate, DateTime? endDate, string reportType)
        {
            var documents = _context.Documents.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                documents = documents.Where(d => d.Title.Contains(searchString) || d.SenderName.Contains(searchString) || d.ReceiverName.Contains(searchString));
            }

            if (startDate.HasValue)
            {
                documents = documents.Where(d => d.CreatedDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                documents = documents.Where(d => d.CreatedDate <= endDate.Value);
            }

            if (!string.IsNullOrEmpty(reportType))
            {
                DateTime currentDate = DateTime.Now;

                switch (reportType)
                {
                    case "Daily":
                        documents = documents.Where(d => d.CreatedDate.Date == currentDate.Date);
                        break;
                    case "Weekly":
                        var currentWeekStart = StartOfWeek(currentDate, DayOfWeek.Monday);
                        var currentWeekEnd = currentWeekStart.AddDays(7);
                        documents = documents.Where(d => d.CreatedDate >= currentWeekStart && d.CreatedDate < currentWeekEnd);
                        break;
                    case "Monthly":
                        var currentMonthStart = new DateTime(currentDate.Year, currentDate.Month, 1);
                        var currentMonthEnd = currentMonthStart.AddMonths(1);
                        documents = documents.Where(d => d.CreatedDate >= currentMonthStart && d.CreatedDate < currentMonthEnd);
                        break;
                }
            }

            var documentList = documents.ToList();

            return GenerateWordDocument(documentList, "DocumentsList.docx");
        }

        private IActionResult GenerateWordDocument(IEnumerable<DocumentManagementSystem.Entities.Document> documents, string fileName)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(memoryStream, WordprocessingDocumentType.Document, true))
                {
                    MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();
                    mainPart.Document = new WordDocument();
                    Body body = mainPart.Document.AppendChild(new Body());

                    SectionProperties sectionProps = new SectionProperties();
                    PageMargin pageMargin = new PageMargin()
                    {
                        Top = 1440,
                        Bottom = 1440,
                        Left = 1440,
                        Right = 1440
                    };
                    sectionProps.Append(pageMargin);
                    body.Append(sectionProps);

                    Paragraph titleParagraph = new Paragraph();
                    Run titleRun = new Run();
                    titleRun.AppendChild(new Text("Document Report"));
                    titleParagraph.AppendChild(titleRun);

                    ParagraphProperties titleProperties = new ParagraphProperties();
                    titleProperties.Append(new Justification() { Val = JustificationValues.Center });
                    RunProperties titleRunProperties = new RunProperties();
                    titleRunProperties.Append(new Bold());
                    titleRunProperties.Append(new FontSize() { Val = "36" });
                    titleRun.PrependChild(titleRunProperties);
                    titleParagraph.PrependChild(titleProperties);

                    body.AppendChild(titleParagraph);
                    body.AppendChild(new Paragraph(new Run(new Text(""))));

                    WordTable table = new WordTable();
                    TableProperties tableProperties = new TableProperties(
                        new TableBorders(
                            new TopBorder() { Val = BorderValues.Single, Size = 8 },
                            new BottomBorder() { Val = BorderValues.Single, Size = 8 },
                            new LeftBorder() { Val = BorderValues.Single, Size = 8 },
                            new RightBorder() { Val = BorderValues.Single, Size = 8 },
                            new InsideHorizontalBorder() { Val = BorderValues.Single, Size = 8 },
                            new InsideVerticalBorder() { Val = BorderValues.Single, Size = 8 }
                        ),
                        new TableWidth() { Width = "5000", Type = TableWidthUnitValues.Pct }
                    );
                    table.AppendChild(tableProperties);

                    TableRow headerRow = new TableRow();
                    headerRow.Append(
                        CreateStyledTableCell("Title", true),
                        CreateStyledTableCell("Type", true),
                        CreateStyledTableCell("Sender", true),
                        CreateStyledTableCell("Receiver", true),
                        CreateStyledTableCell("Status", true),
                        CreateStyledTableCell("Created Date", true)
                    );
                    table.AppendChild(headerRow);

                    foreach (var doc in documents)
                    {
                        TableRow dataRow = new TableRow();
                        dataRow.Append(
                            CreateStyledTableCell(doc.Title),
                            CreateStyledTableCell(doc.TypeOfDoc),
                            CreateStyledTableCell(doc.SenderName),
                            CreateStyledTableCell(doc.ReceiverName),
                            CreateStyledTableCell(doc.DocStatus.ToString()),
                            CreateStyledTableCell(doc.CreatedDate.ToString("MM/dd/yyyy h:mm tt"))
                        );
                        table.AppendChild(dataRow);
                    }

                    body.AppendChild(table);
                    mainPart.Document.Save();
                }

                memoryStream.Position = 0;
                return File(memoryStream.ToArray(), "application/vnd.openxmlformats-officedocument.wordprocessingml.document", fileName);
            }
        }

        private TableCell CreateStyledTableCell(string text, bool isHeader = false)
        {
            TableCell cell = new TableCell();
            Paragraph paragraph = new Paragraph();
            Run run = new Run();
            run.AppendChild(new Text(text));

            RunProperties runProperties = new RunProperties();
            runProperties.Append(new FontSize() { Val = "24" });
            if (isHeader)
            {
                runProperties.Append(new Bold());
            }
            run.PrependChild(runProperties);
            paragraph.AppendChild(run);

            ParagraphProperties paragraphProperties = new ParagraphProperties();
            paragraphProperties.Append(new Justification() { Val = JustificationValues.Center });
            paragraph.PrependChild(paragraphProperties);

            cell.AppendChild(paragraph);
            return cell;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
