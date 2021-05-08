using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Reports.Models;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reports.Services.Helper
{
    public class ReportBuilder : IReportBuilder
    {
        private readonly string ApplicationPath = Directory.GetCurrentDirectory() + "\\SourceData\\Fonts\\";

        private IEnumerable<string[]> ReadFile(string filePath)
        {
            var lines = File.ReadAllLines(filePath, Encoding.UTF8).Select(a => a.Split(";"));
            return lines;
        }

        private DataTable GetDefaultDataTable(string filePath)
        {

            var dataTable = new DataTable();
            dataTable.TableName = "CsvFile";

            dataTable.Columns.AddRange(new DataColumn[5] 
            {
                new DataColumn("ID", typeof(string)),
                new DataColumn("Name", typeof(string)),
                new DataColumn("Adress", typeof(string)),
                new DataColumn("Service", typeof(string)),
                new DataColumn("Price", typeof(string))
            });

            foreach (var row in ReadFile(filePath))
                if (row.Length == 5)
                    dataTable.Rows.Add(row[0], row[1], row[2], row[3], row[4]);

            return dataTable;
        }

        private bool dataIsCorrect(DataTable dataTable)
        {
            var headerRow = dataTable.Rows[0];

            if (headerRow[0].ToString() != "ID")
                return false;

            if (headerRow[1].ToString() != "Название")
                return false;

            if (headerRow[2].ToString() != "Адрес")
                return false;

            if (headerRow[3].ToString() != "Услуга")
                return false;

            if (headerRow[4].ToString() != "Стоимость")
                return false;

            return true;
        }

        public async Task<DefaultResponse> DefaultSaveAsExcel(Entities.User user, Entities.File file, Entities.Report report)
        {

            var reportData = GetDefaultDataTable(file.Path + file.Name);

            if (reportData.Rows.Count <= 1)
                return new DefaultResponse()
                {
                    Status = "Error",
                    Message = "The uploaded аile is empty or no data filled!",
                    Done = false
                };

            if (!dataIsCorrect(reportData))
                return new DefaultResponse()
                {
                    Status = "Error",
                    Message = "The data in the uploaded file does not match the pattern!",
                    Done = false
                };

            using (var woekBook = new XLWorkbook())
            {
                var worksheet = woekBook.Worksheets.Add("Report");

                worksheet.Cell(3, 5).Value = "Auto-Generated Report";
                worksheet.Cell(3, 5).Style.Font.Bold = true;
                worksheet.Cell(3, 5).Style.Font.FontSize = 16;
                worksheet.Cell(3, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                worksheet.Cell(5, 4).Value = "Report";
                worksheet.Cell(5, 4).Style.Font.Bold = true;
                worksheet.Cell(5, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                worksheet.Cell(6, 4).Value = "Name:";
                worksheet.Cell(6, 4).Style.Font.Bold = true;
                worksheet.Cell(6, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                worksheet.Cell(6, 5).Value = report.Name;
                worksheet.Cell(6, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                worksheet.Cell(7, 4).Value = "From file:";
                worksheet.Cell(7, 4).Style.Font.Bold = true;
                worksheet.Cell(7, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                worksheet.Cell(7, 5).Value = file.Name;
                worksheet.Cell(7, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                worksheet.Cell(8, 4).Value = "Created:";
                worksheet.Cell(8, 4).Style.Font.Bold = true;
                worksheet.Cell(8, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                worksheet.Cell(8, 5).Value = report.DateCreated;
                worksheet.Cell(8, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                worksheet.Cell(9, 4).Value = "Owner";
                worksheet.Cell(9, 4).Style.Font.Bold = true;
                worksheet.Cell(9, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                worksheet.Cell(10, 4).Value = "Surname:";
                worksheet.Cell(10, 4).Style.Font.Bold = true;
                worksheet.Cell(10, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                worksheet.Cell(10, 5).Value = user.Surname;
                worksheet.Cell(10, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                worksheet.Cell(11, 4).Value = "Name:";
                worksheet.Cell(11, 4).Style.Font.Bold = true;
                worksheet.Cell(11, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                worksheet.Cell(11, 5).Value = user.Name;
                worksheet.Cell(11, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                worksheet.Cell(13, 3).Value = reportData.Rows[0][0];
                worksheet.Cell(13, 3).Style.Font.Bold = true;
                worksheet.Cell(13, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                worksheet.Cell(13, 4).Value = reportData.Rows[0][1];
                worksheet.Cell(13, 4).Style.Font.Bold = true;
                worksheet.Cell(13, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                worksheet.Cell(13, 5).Value = reportData.Rows[0][2];
                worksheet.Cell(13, 5).Style.Font.Bold = true;
                worksheet.Cell(13, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                worksheet.Cell(13, 6).Value = reportData.Rows[0][3];
                worksheet.Cell(13, 6).Style.Font.Bold = true;
                worksheet.Cell(13, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                worksheet.Cell(13, 7).Value = reportData.Rows[0][4];
                worksheet.Cell(13, 7).Style.Font.Bold = true;
                worksheet.Cell(13, 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                reportData.Rows.Remove(reportData.Rows[0]);

                worksheet.Cell(14, 3).Value = reportData.Rows;

                worksheet.Columns().AdjustToContents();

                woekBook.SaveAs(report.Path + report.Name);
            }

            return new DefaultResponse()
            {
                Status = "Success",
                Message = "The report saved successfylly! (Excel)",
                Done = true
            };
        }

        public async Task<DefaultResponse> DefaultSaveAsPdf(Entities.User user, Entities.File file, Entities.Report report)
        {
            var reportData = GetDefaultDataTable(file.Path + file.Name);

            if (reportData.Rows.Count <= 1)
                return new DefaultResponse()
                {
                    Status = "Error",
                    Message = "The uploaded аile is empty or no data filled!",
                    Done = false
                };

            if (!dataIsCorrect(reportData))
                return new DefaultResponse()
                {
                    Status = "Error",
                    Message = "The data in the uploaded file does not match the pattern!",
                    Done = false
                };

            var document = new Document(PageSize.A4, 5f, 5f, 10f, 10f);
            var fileStream = new FileStream(report.Path + report.Name, FileMode.Create);
            var pdfWriter = PdfWriter.GetInstance(document, fileStream);
            document.Open();

            var encodingProvider = CodePagesEncodingProvider.Instance;
            Encoding.RegisterProvider(encodingProvider);

            var baseFont = BaseFont.CreateFont(ApplicationPath + "\\arial.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            var fontDefaultNNormal = new Font(baseFont, Font.DEFAULTSIZE, Font.NORMAL);
            var fontDefaultBold = new Font(baseFont, Font.DEFAULTSIZE, Font.BOLD);
            var fontHeader = new Font(baseFont, 16, Font.BOLD);

            float[] columnReportInfoDefinitionSize = { 2F, 2F, 2F, 2F, 2F, 2F };

            PdfPTable tableReportInfo;
            PdfPCell cellReportInfo;

            tableReportInfo = new PdfPTable(columnReportInfoDefinitionSize);

            cellReportInfo = new PdfPCell
            {
                BackgroundColor = new BaseColor(0xC0, 0xC0, 0xC0)
            };

            cellReportInfo = new PdfPCell(new Phrase("Auto-Generated Report", fontHeader));
            cellReportInfo.Colspan = 6;
            cellReportInfo.HorizontalAlignment = Element.ALIGN_CENTER;
            cellReportInfo.Border = Rectangle.NO_BORDER;
            tableReportInfo.AddCell(cellReportInfo);

            cellReportInfo = new PdfPCell(new Phrase("Report: ", fontDefaultBold));
            cellReportInfo.Colspan = 6;
            cellReportInfo.HorizontalAlignment = Element.ALIGN_LEFT;
            cellReportInfo.Border = Rectangle.NO_BORDER;
            tableReportInfo.AddCell(cellReportInfo);

            cellReportInfo = new PdfPCell(new Phrase("Name: ", fontDefaultBold));
            cellReportInfo.Colspan = 1;
            cellReportInfo.HorizontalAlignment = Element.ALIGN_RIGHT;
            cellReportInfo.Border = Rectangle.NO_BORDER;
            tableReportInfo.AddCell(cellReportInfo);

            cellReportInfo = new PdfPCell(new Phrase(report.Name, fontDefaultNNormal));
            cellReportInfo.Colspan = 5;
            cellReportInfo.HorizontalAlignment = Element.ALIGN_LEFT;
            cellReportInfo.Border = Rectangle.NO_BORDER;
            tableReportInfo.AddCell(cellReportInfo);

            cellReportInfo = new PdfPCell(new Phrase("From file: ", fontDefaultBold));
            cellReportInfo.Colspan = 1;
            cellReportInfo.HorizontalAlignment = Element.ALIGN_RIGHT;
            cellReportInfo.Border = Rectangle.NO_BORDER;
            tableReportInfo.AddCell(cellReportInfo);

            cellReportInfo = new PdfPCell(new Phrase(file.Name, fontDefaultNNormal));
            cellReportInfo.Colspan = 5;
            cellReportInfo.HorizontalAlignment = Element.ALIGN_LEFT;
            cellReportInfo.Border = Rectangle.NO_BORDER;
            tableReportInfo.AddCell(cellReportInfo);

            cellReportInfo = new PdfPCell(new Phrase("Created: ", fontDefaultBold));
            cellReportInfo.Colspan = 1;
            cellReportInfo.HorizontalAlignment = Element.ALIGN_RIGHT;
            cellReportInfo.Border = Rectangle.NO_BORDER;
            tableReportInfo.AddCell(cellReportInfo);

            cellReportInfo = new PdfPCell(new Phrase("" + report.DateCreated, fontDefaultNNormal));
            cellReportInfo.Colspan = 5;
            cellReportInfo.HorizontalAlignment = Element.ALIGN_LEFT;
            cellReportInfo.Border = Rectangle.NO_BORDER;
            tableReportInfo.AddCell(cellReportInfo);

            cellReportInfo = new PdfPCell(new Phrase("Owner: ", fontDefaultBold));
            cellReportInfo.Colspan = 6;
            cellReportInfo.HorizontalAlignment = Element.ALIGN_LEFT;
            cellReportInfo.Border = Rectangle.NO_BORDER;
            tableReportInfo.AddCell(cellReportInfo);

            cellReportInfo = new PdfPCell(new Phrase("Surname: ", fontDefaultBold));
            cellReportInfo.Colspan = 1;
            cellReportInfo.HorizontalAlignment = Element.ALIGN_RIGHT;
            cellReportInfo.Border = Rectangle.NO_BORDER;
            tableReportInfo.AddCell(cellReportInfo);

            cellReportInfo = new PdfPCell(new Phrase(user.Surname, fontDefaultNNormal));
            cellReportInfo.Colspan = 5;
            cellReportInfo.HorizontalAlignment = Element.ALIGN_LEFT;
            cellReportInfo.Border = Rectangle.NO_BORDER;
            tableReportInfo.AddCell(cellReportInfo);

            cellReportInfo = new PdfPCell(new Phrase("Name: ", fontDefaultBold));
            cellReportInfo.Colspan = 1;
            cellReportInfo.HorizontalAlignment = Element.ALIGN_RIGHT;
            cellReportInfo.Border = Rectangle.NO_BORDER;
            tableReportInfo.AddCell(cellReportInfo);

            cellReportInfo = new PdfPCell(new Phrase(user.Name));
            cellReportInfo.Colspan = 5;
            cellReportInfo.HorizontalAlignment = Element.ALIGN_LEFT;
            cellReportInfo.Border = Rectangle.NO_BORDER;
            tableReportInfo.AddCell(cellReportInfo);

            cellReportInfo = new PdfPCell(new Phrase(" "));
            cellReportInfo.Colspan = 6;
            cellReportInfo.Border = Rectangle.NO_BORDER;
            tableReportInfo.AddCell(cellReportInfo);

            float[] columnDataFromFileDefinitionSize = { 1F, 3F, 4F, 5F, 3F };
            PdfPTable tableDataFromFile;
            PdfPCell cellDataFromFile;

            tableDataFromFile = new PdfPTable(columnDataFromFileDefinitionSize);

            cellDataFromFile = new PdfPCell(new Phrase("ID", fontDefaultBold));
            cellDataFromFile.VerticalAlignment = Element.ALIGN_CENTER;
            cellDataFromFile.HorizontalAlignment = Element.ALIGN_CENTER;
            tableDataFromFile.AddCell(cellDataFromFile);

            cellDataFromFile = new PdfPCell(new Phrase("Название", fontDefaultBold));
            cellDataFromFile.VerticalAlignment = Element.ALIGN_CENTER;
            cellDataFromFile.HorizontalAlignment = Element.ALIGN_CENTER;
            tableDataFromFile.AddCell(cellDataFromFile);

            cellDataFromFile = new PdfPCell(new Phrase("Адрес", fontDefaultBold));
            cellDataFromFile.VerticalAlignment = Element.ALIGN_CENTER;
            cellDataFromFile.HorizontalAlignment = Element.ALIGN_CENTER;
            tableDataFromFile.AddCell(cellDataFromFile);

            cellDataFromFile = new PdfPCell(new Phrase("Услуга", fontDefaultBold));
            cellDataFromFile.VerticalAlignment = Element.ALIGN_CENTER;
            cellDataFromFile.HorizontalAlignment = Element.ALIGN_CENTER;
            tableDataFromFile.AddCell(cellDataFromFile);

            cellDataFromFile = new PdfPCell(new Phrase("Стоимость", fontDefaultBold));
            cellDataFromFile.VerticalAlignment = Element.ALIGN_CENTER;
            cellDataFromFile.HorizontalAlignment = Element.ALIGN_CENTER;
            tableDataFromFile.AddCell(cellDataFromFile);

            int currentRow = 0;

            foreach (DataRow data in reportData.Rows)
            {
                if (currentRow != 0)
                {
                    tableDataFromFile.AddCell(new Phrase(data["ID"].ToString(), fontDefaultNNormal));
                    tableDataFromFile.AddCell(new Phrase(data["Name"].ToString(), fontDefaultNNormal));
                    tableDataFromFile.AddCell(new Phrase(data["Adress"].ToString(), fontDefaultNNormal));
                    tableDataFromFile.AddCell(new Phrase(data["Service"].ToString(), fontDefaultNNormal));
                    tableDataFromFile.AddCell(new Phrase(data["Price"].ToString(), fontDefaultNNormal));
                }

                currentRow++;
            }

            document.Add(tableReportInfo);
            document.Add(tableDataFromFile);
            document.Close();
            document.CloseDocument();
            document.Dispose();
            pdfWriter.Close();
            pdfWriter.Dispose();
            fileStream.Close();
            fileStream.Dispose();

            return new DefaultResponse()
            {
                Status = "Success",
                Message = "The report saved successfylly! (PDF)",
                Done = true
            };
        }
    }
}
