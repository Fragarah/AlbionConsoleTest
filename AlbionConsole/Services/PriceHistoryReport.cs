using AlbionConsole.Models;
using QuestPDF.Infrastructure;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace AlbionConsole.Services
{
    public class PriceHistoryReport : IDocument
    {
        private readonly Dictionary<Item, List<PriceHistory>> _data;

        public PriceHistoryReport(Dictionary<Item, List<PriceHistory>> data)
        {
            _data = data;
        }
        public PriceHistoryReport()
        {
            // Konstruktor bez parametrów, jeśli potrzebny
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10));

                // HEADER
                page.Header().Element(header =>
                {
                    header.Element(c =>
                    {
                        c.PaddingBottom(10)
                         .Text("Raport Historycznych Cen")
                         .FontSize(18)
                         .Bold()
                         .AlignCenter();
                    });
                });

                // CONTENT
                page.Content().Column(col =>
                {
                    foreach (var kvp in _data)
                    {
                        var item = kvp.Key;
                        var history = kvp.Value
                            .OrderByDescending(x => x.Timestamp)
                            .Take(30)
                            .OrderBy(x => x.Timestamp)
                            .ToList();

                        // Nazwa przedmiotu
                        col.Item().Element(c =>
                        {
                            c.PaddingVertical(10)
                             .Text($"Przedmiot: {item.Name}")
                             .FontSize(14)
                             .Bold();
                        });

                        // Tabela danych
                        col.Item().Table(table =>
                        {
                            // Kolumny
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();    // Miasto
                                columns.ConstantColumn(40);  // Jakość
                                columns.ConstantColumn(60);  // Sell Min
                                columns.ConstantColumn(60);  // Sell Max
                                columns.ConstantColumn(60);  // Buy Min
                                columns.ConstantColumn(60);  // Buy Max
                                columns.RelativeColumn();    // Data
                            });

                            // Nagłówki
                            table.Header(header =>
                            {
                                header.Cell().Element(c => c.Text("Miasto").Bold());
                                header.Cell().Element(c => c.Text("Jakość").Bold());
                                header.Cell().Element(c => c.Text("Sprz. Min").Bold());
                                header.Cell().Element(c => c.Text("Sprz. Max").Bold());
                                header.Cell().Element(c => c.Text("Kupno Min").Bold());
                                header.Cell().Element(c => c.Text("Kupno Max").Bold());
                                header.Cell().Element(c => c.Text("Data").Bold());
                            });

                            // Wiersze
                            foreach (var entry in history)
                            {
                                table.Cell().Element(c => c.Text(entry.Location));
                                table.Cell().Element(c => c.Text(entry.Quality.ToString()));
                                table.Cell().Element(c => c.Text(entry.SellPriceMin.ToString()));
                                table.Cell().Element(c => c.Text(entry.SellPriceMax.ToString()));
                                table.Cell().Element(c => c.Text(entry.BuyPriceMin.ToString()));
                                table.Cell().Element(c => c.Text(entry.BuyPriceMax.ToString()));
                                table.Cell().Element(c => c.Text(entry.Timestamp.ToString("yyyy-MM-dd")));
                            }
                        });
                    }
                });

                // FOOTER
                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Wygenerowano: ");
                    x.Span(DateTime.Now.ToString("g")).SemiBold();
                });
            });
        }
    }
}
