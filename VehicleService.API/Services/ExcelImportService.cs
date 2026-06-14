using OfficeOpenXml;
using VehicleService.API.Data;
using VehicleService.API.Models;
using System.Globalization;

namespace VehicleService.API.Services
{
    public class ExcelImportService
    {
        private readonly AppDbContext _context;

        public ExcelImportService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> ImportAsync(IFormFile file)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);

            using var package = new ExcelPackage(stream);
            var sheet = package.Workbook.Worksheets[0];

            int rowCount = sheet.Dimension.Rows;
            int colCount = sheet.Dimension.Columns;

            int headerRow = FindHeaderRow(sheet, rowCount, colCount);

            var columns = GetColumnMap(sheet, headerRow, colCount);

            int importedCount = 0;

            for (int row = headerRow + 1; row <= rowCount; row++)
            {
                var documentNumber = GetCell(sheet, row, columns, "№ документа");
                var brand = GetCell(sheet, row, columns, "Марка");
                var vin = GetCell(sheet, row, columns, "VIN");

                if (string.IsNullOrWhiteSpace(documentNumber) &&
                    string.IsNullOrWhiteSpace(brand) &&
                    string.IsNullOrWhiteSpace(vin))
                {
                    continue;
                }

                var inspection = new VehicleInspection
                {
                    VehicleCompliance = GetCell(sheet, row, columns, "Відповідність ТЗ"),

                    DocumentNumber = documentNumber,
                    BlankNumber = GetCell(sheet, row, columns, "Серія, номер бланку"),
                    Status = GetCell(sheet, row, columns, "Статус документа"),

                    SignDate = ParseDate(sheet.Cells[row, columns["Дата підпису"]]),
                    SignTime = ParseTime(sheet.Cells[row, columns["Час підпису"]]),

                    VehicleType = GetCell(sheet, row, columns, "Тип ТЗ"),
                    Brand = brand,
                    Model = GetCell(sheet, row, columns, "Модель"),
                    Category = GetCell(sheet, row, columns, "Категорія"),

                    PlateNumber = GetCell(sheet, row, columns, "Державний номер"),
                    VIN = vin,

                    Owner = GetCell(sheet, row, columns, "Власник"),
                    OwnerType = GetCell(sheet, row, columns, "Тип власника"),

                    IsSubjectToInspection = GetCell(sheet, row, columns, "Підлягає ОТК") == "Так",
                    InspectionPeriod = GetCell(sheet, row, columns, "Періодичність проходження ОТК"),

                    NextInspectionDate = ParseNullableDate(sheet.Cells[row, columns["Дата наступного ОТК"]]),

                    IsInternational = GetCell(sheet, row, columns, "Міжнародний ТО") == "Так",
                    IsCanceled = GetCell(sheet, row, columns, "Анульований") == "Так"
                };

                _context.Inspections.Add(inspection);
                importedCount++;
            }

            await _context.SaveChangesAsync();

            return importedCount;
        }

        private int FindHeaderRow(ExcelWorksheet sheet, int rowCount, int colCount)
        {
            for (int row = 1; row <= rowCount; row++)
            {
                for (int col = 1; col <= colCount; col++)
                {
                    if (sheet.Cells[row, col].Text.Trim() == "№ документа")
                        return row;
                }
            }

            throw new Exception("Не знайдено рядок заголовків Excel.");
        }

        private Dictionary<string, int> GetColumnMap(ExcelWorksheet sheet, int headerRow, int colCount)
        {
            var map = new Dictionary<string, int>();

            for (int col = 1; col <= colCount; col++)
            {
                var header = sheet.Cells[headerRow, col].Text.Trim();

                if (!string.IsNullOrWhiteSpace(header) && !map.ContainsKey(header))
                    map.Add(header, col);
            }

            return map;
        }

        private string GetCell(ExcelWorksheet sheet, int row, Dictionary<string, int> columns, string columnName)
        {
            return columns.ContainsKey(columnName)
                ? sheet.Cells[row, columns[columnName]].Text.Trim()
                : string.Empty;
        }

        private DateTime ParseDate(ExcelRange cell)
        {
            if (cell.Value is DateTime date)
                return date;

            var text = cell.Text.Trim();

            return DateTime.TryParseExact(
                text,
                "dd.MM.yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var result)
                ? result
                : DateTime.MinValue;
        }

        private DateTime? ParseNullableDate(ExcelRange cell)
        {
            if (cell.Value is DateTime date)
                return date;

            var text = cell.Text.Trim();

            if (string.IsNullOrWhiteSpace(text) || text == "-")
                return null;

            return DateTime.TryParseExact(
                text,
                "dd.MM.yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var result)
                ? result
                : null;
        }

        private TimeSpan ParseTime(ExcelRange cell)
        {
            var text = cell.Text.Trim();

            return TimeSpan.TryParse(text, out var result)
                ? result
                : TimeSpan.Zero;
        }
    }
}