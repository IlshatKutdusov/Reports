using ClosedXML.Excel;
using iTextSharp;
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

        private DataTable GetDataTable(string filePath)
        {

            var dataTable = new DataTable();
            dataTable.TableName = "CsvFile";

            dataTable.Columns.AddRange(new DataColumn[4] 
            {
                new DataColumn("ID", typeof(string)),
                new DataColumn("Name", typeof(string)),
                new DataColumn("Adress", typeof(string)),
                new DataColumn("Value", typeof(string))
            });

            foreach (var row in ReadFile(filePath))
                dataTable.Rows.Add(row[0], row[1], row[2], row[3]);

            return dataTable;
        }

        public async Task<DefaultResponse> SaveAsExcel(Entities.User user, Entities.File file, Entities.Report report)
        {

            var reportData = GetDataTable(file.Path + file.Name);

            using (var woekBook = new XLWorkbook())
            {
                woekBook.Worksheets.Add(reportData);
                woekBook.SaveAs(report.Path + report.Name);
            }

            return new DefaultResponse()
            {

            };
        }

        public async Task<DefaultResponse> SaveAsPdf(Entities.User user, Entities.File file, Entities.Report report)
        {
            var reportData = GetDataTable(file.Path + file.Name);

            if (reportData.Rows.Count > 0)
            {
                var document = new Document(PageSize.A4, 5f, 5f, 10f, 10f);
                var fileStream = new FileStream(report.Path + report.Name, FileMode.Create);
                var pdfWriter = PdfWriter.GetInstance(document, fileStream);
                document.Open();

                System.Text.EncodingProvider ppp = System.Text.CodePagesEncodingProvider.Instance;
                Encoding.RegisterProvider(ppp);

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

                float[] columnDataFromFileDefinitionSize = { 1F, 4F, 6F, 3F };
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

                cellDataFromFile = new PdfPCell(new Phrase("Показатель", fontDefaultBold));
                cellDataFromFile.VerticalAlignment = Element.ALIGN_CENTER;
                cellDataFromFile.HorizontalAlignment = Element.ALIGN_CENTER;
                tableDataFromFile.AddCell(cellDataFromFile);

                //tableDataFromFile.HeaderRows = 1;


                int currentRow = 0;

                foreach (DataRow data in reportData.Rows)
                {
                    if (currentRow != 0)
                    {
                        tableDataFromFile.AddCell(new Phrase(data["ID"].ToString(), fontDefaultNNormal));
                        tableDataFromFile.AddCell(new Phrase(data["Name"].ToString(), fontDefaultNNormal));
                        tableDataFromFile.AddCell(new Phrase(data["Adress"].ToString(), fontDefaultNNormal));
                        tableDataFromFile.AddCell(new Phrase(data["Value"].ToString(), fontDefaultNNormal)); 
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
            }

            return new DefaultResponse()
            {

            };
        }
    }
}
