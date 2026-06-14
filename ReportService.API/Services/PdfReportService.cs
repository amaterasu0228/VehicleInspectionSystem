using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ReportService.API.DTOs;

namespace ReportService.API.Services
{
    public class PdfReportService
    {
        public byte[] GenerateAnalyticsReport(AnalyticsReportDto report)
        {
            var pdf = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(40);
                    page.Size(PageSizes.A4);

                    page.Header().Column(column =>
                    {
                        column.Item().Text("ІНФОРМАЦІЙНО-АНАЛІТИЧНА СИСТЕМА")
                            .FontSize(16)
                            .Bold()
                            .AlignCenter();

                        column.Item().Text("автоматизації технічного огляду транспортних засобів")
                            .FontSize(12)
                            .AlignCenter();

                        column.Item().PaddingTop(10).Text("АНАЛІТИЧНИЙ ЗВІТ")
                            .FontSize(18)
                            .Bold()
                            .AlignCenter();

                        column.Item().PaddingTop(10).LineHorizontal(1);
                    });

                    page.Content().PaddingVertical(20).Column(column =>
                    {
                        column.Spacing(14);

                        column.Item().Text($"Період аналізу: {report.DateFrom} - {report.DateTo}")
                            .FontSize(12)
                            .Bold();

                        AddSection(column, "1. Загальна статистика", new Dictionary<string, string>
                        {
                            ["Проведено ТО"] = report.TotalInspections.ToString(),
                            ["Міжнародних ТО"] = report.InternationalInspections.ToString(),
                            ["Актів невідповідності"] = report.NonComplianceActs.ToString(),
                            ["Анульованих документів"] = report.CanceledCount.ToString()
                        });

                        AddSection(column, "2. Статистика клієнтів", new Dictionary<string, string>
                        {
                            ["Усього клієнтів"] = report.ClientsCount.ToString(),
                            ["Нових клієнтів"] = report.NewClientsCount.ToString()
                        });

                        AddSection(column, "3. Статистика заявок", new Dictionary<string, string>
                        {
                            ["Усього заявок"] = report.TotalAppointments.ToString(),
                            ["Нових"] = report.NewAppointments.ToString(),
                            ["Підтверджених"] = report.ConfirmedAppointments.ToString(),
                            ["Виконаних"] = report.CompletedAppointments.ToString()
                        });

                        AddSection(column, "4. Нагадування про закінчення ОТК", new Dictionary<string, string>
                        {
                            ["Активних нагадувань"] = report.ActiveReminders.ToString(),
                            ["Прочитаних нагадувань"] = report.ReadReminders.ToString()
                        });

                        
                        AddBarChartSection(
                            column,
                            "5. Розподіл за типами ТЗ",
                            report.VehicleTypeStats.Select(x => (x.Name, x.Count)).ToList()
                        );

                        AddBarChartSection(
                            column,
                            "6. Розподіл за категоріями",
                            report.CategoryStats.Select(x => (x.Name, x.Count)).ToList()
                        );

                        column.Item()
    .PaddingTop(15)
    .Text("Висновок")
    .FontSize(14)
    .Bold();

                        column.Item().Text(
                            $"За обраний період виконано {report.TotalInspections} технічних оглядів. " +
                            $"Кількість міжнародних технічних оглядів становить {report.InternationalInspections}, " +
                            $"кількість актів невідповідності — {report.NonComplianceActs}, " +
                            $"а кількість зареєстрованих клієнтів становить {report.ClientsCount}. " +
                            $"Система забезпечує автоматизований облік результатів технічного контролю, формування статистичної звітності та моніторинг термінів дії технічних оглядів."
                        );

                        column.Item().PaddingTop(10).Text("Звіт сформовано системою автоматично.")
                            .Italic();

                        column.Item().Text($"Користувач: {report.GeneratedBy}");
                        column.Item().Text($"Дата формування: {DateTime.Now:dd.MM.yyyy HH:mm}");
                    });

                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.Span("VehicleInspectionSystem • ");
                        text.CurrentPageNumber();
                        text.Span(" / ");
                        text.TotalPages();
                    });
                });
            });

            return pdf.GeneratePdf();
        }

        private void AddSection(ColumnDescriptor column, string title, Dictionary<string, string> data)
        {
            column.Item().PaddingTop(8).Text(title)
                .FontSize(14)
                .Bold();

            column.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(3);
                    columns.RelativeColumn(1);
                });

                foreach (var item in data)
                {
                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(6).Text(item.Key);
                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(6).AlignRight().Text(item.Value).Bold();
                }
            });
        }

        private void AddBarChartSection(
    ColumnDescriptor column,
    string title,
    List<(string Label, int Count)> data)
        {
            column.Item().PaddingTop(10).Text(title)
                .FontSize(14)
                .Bold();

            if (data == null || data.Count == 0)
            {
                column.Item().Text("Дані відсутні.");
                return;
            }

            var max = data.Max(x => x.Count);

            if (max <= 0)
            {
                column.Item().Text("Дані відсутні.");
                return;
            }

            foreach (var item in data.Take(12))
            {
                var widthPercent = (float)item.Count / max;

                if (widthPercent <= 0)
                    widthPercent = 0.01f;

                column.Item().PaddingVertical(3).Column(row =>
                {
                    row.Item().Text($"{item.Label} — {item.Count}")
                        .FontSize(10);

                    row.Item()
                        .Height(10)
                        .Background(Colors.Grey.Lighten3)
                        .Row(r =>
                        {
                            r.RelativeItem(widthPercent)
                                .Background(Colors.Blue.Medium);

                            if (widthPercent < 1)
                            {
                                r.RelativeItem(1 - widthPercent);
                            }
                        });
                });
            }
        }
    }
}